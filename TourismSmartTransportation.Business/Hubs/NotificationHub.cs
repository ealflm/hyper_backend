using GeoCoordinatePortable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Hubs.Mapping;
using TourismSmartTransportation.Business.Hubs.Models;
using TourismSmartTransportation.Business.Hubs.Store;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.Order;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.OrderDetail;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        // Inject service
        private readonly ILogger<NotificationHub> _logger;
        private readonly IDriverManagementService _driverService;
        private readonly IOrderHelpersService _orderHelperService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingService _bookingService;

        // Global parameters to store data
        private readonly List<double> DISTANCES = new List<double>() { 3000, 5000, 7000, 10000 };
        private readonly static VehicleStore _vehicleStore = new VehicleStore();

        // Mapping data store
        private readonly static SearchMapping _searchMapping = new SearchMapping();
        private readonly static StoreDataMappingWithMultipleKeys<string, string> _driverDeclineList = new StoreDataMappingWithMultipleKeys<string, string>(); // tạo instance lưu danh sách driver từ chối cuốc của khách hàng
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>(); // Tạo instance connection mapping
        private readonly static DataMapping _dataMapping = new DataMapping(); // Tạo instance lưu danh sách thông tin customer và danh sách thông tin driver
        private readonly static RoomMapping _roomMapping = new RoomMapping(); // Tạo instance chứa driver và customer khi đã booking 

        public NotificationHub(ILogger<NotificationHub> logger,
                                IUnitOfWork unitOfWork,
                                IDriverManagementService driverService,
                                IOrderHelpersService orderHelpersService,
                                IBookingService bookingService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _driverService = driverService;
            _orderHelperService = orderHelpersService;
            _bookingService = bookingService;
        }

        private bool CompareTime(DateTime date, Time time = Time.Day)
        {
            var day = DateTime.UtcNow.Day;
            var month = DateTime.UtcNow.Month;
            var year = DateTime.UtcNow.Year;

            DateTime start = new DateTime(year, month, day);
            dynamic end = new DateTime(year, month, day).AddDays(1);
            if (time == Time.Month)
            {
                end = new DateTime(year, month, day).AddMonths(1);
            }

            if (start.CompareTo(date) <= 0 && date.CompareTo(end) < 0)
            {
                return true;
            }

            return false;
        }

        public Expression<Func<CustomerTrip, bool>> CompareTime(Time time = Time.Day)
        {
            var day = DateTime.UtcNow.Day;
            var month = DateTime.UtcNow.Month;
            var year = DateTime.UtcNow.Year;

            DateTime start = new DateTime(year, month, day);
            DateTime end = new DateTime(year, month, day).AddDays(1);
            if (time == Time.Month)
            {
                end = new DateTime(year, month, day).AddMonths(1);
            }
            return x => start.CompareTo(x.CreatedDate) <= 0 && x.CreatedDate.CompareTo(end) < 0;
        }

        public async Task FindDriver(string json, string isClient = "true")
        {
            _logger.LogInformation("---------------- Begin FindDriver Hub Function ------------------");
            _logger.LogInformation($"---------------- JSON FINDDRIVER {json} ------------------");

            LocationModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<LocationModel>(json); // parse json to object
            DataHubModel customer = _dataMapping.GetValue(model.Id, User.Customer);
            if (customer != null)
            {
                customer.Longitude = model.Longitude;
                customer.Latitude = model.Latitude;
                customer.PriceBookingId = model.PriceBookingId;
                customer.Price = model.Price;
                customer.Distance = model.Distance;
                customer.Seats = model.Seats;
                customer.IntervalLoopIndex = isClient == "true" ? 0 : customer.IntervalLoopIndex >= DISTANCES.Count - 1 ? 0 : customer.IntervalLoopIndex;
                customer.Status = isClient == "true" ? (int)CustomerStatus.Normal : customer.Status;
                _dataMapping.Add(model.Id, customer, User.Customer);
            }

            if (customer.Status == (int)CustomerStatus.NotFound || customer.Status == (int)CustomerStatus.OnDisconnect)
            {
                customer.IntervalLoopIndex = 0;
                _dataMapping.Add(model.Id, customer, User.Customer);
                _logger.LogInformation($"---------- NOT FOUND DRIVER OR DISCONNECT ---------------");
                return;
            }

            bool isFoundDriver = false;

            // if (customer.IntervalLoopIndex == DISTANCES.Count - 1 && customer.Status == (int)CustomerStatus.NotFound)
            // {
            //     customer.IntervalLoopIndex = 0;
            // }

            for (int i = customer.IntervalLoopIndex; i < DISTANCES.Count; i++) // Tìm kiếm trong những khoảng distance 3000m 5000m 7000m
            {
                _logger.LogInformation($"---------- Go to IntervalLoopIndex - {i} ---------------");

                customer.Status = (int)CustomerStatus.Finding;
                customer.IntervalLoopIndex = i >= DISTANCES.Count - 1
                                                    ? (int)DISTANCES[DISTANCES.Count - 1]
                                                    : i;

                _dataMapping.Add(model.Id, customer, User.Customer);

                foreach (var item in _dataMapping.GetDrivers(DriverStatus.On).GetItems())
                {
                    _logger.LogInformation($"---------- DriverID::: {item.Value.Id} ---------------");
                    _logger.LogInformation($"---------- DriverNAME::: {item.Value.LastName} - {item.Value.FirstName} ---------------");
                    _logger.LogInformation($"---------- GO TO FIND MATCHING DRIVER  ---------------");

                    // kiểm tra xem tài xế này đã từ chối trước đó hay chưa
                    var isExisted = _driverDeclineList.CheckExistedValue(customer.Id, item.Key);
                    if (isExisted)
                    {
                        continue;
                    }

                    var driverId = item.Key;
                    var lng = (double)item.Value.Longitude;
                    var lat = (double)item.Value.Latitude;
                    GeoCoordinate customerCoordinates = new GeoCoordinate((double)model.Latitude, (double)model.Longitude);
                    GeoCoordinate driverCoordinates = new GeoCoordinate(lat, lng);
                    double distanceBetween = customerCoordinates.GetDistanceTo(driverCoordinates); // tính khoảng cách giữa driver và customer


                    _logger.LogInformation($"---------- distanceBetween {distanceBetween} ---------------");

                    var d = await _unitOfWork.DriverRepository.GetById(Guid.Parse(driverId)); // lấy thông tin driver
                    if (d == null)
                    {
                        continue;
                    }

                    var vehicle = _vehicleStore.VehiclesList.GetValueOrDefault(d.VehicleId.Value.ToString()); // lấy thông tin xe mà driver đang được cấp
                    if (vehicle == null)
                    {
                        continue;
                    }

                    var vehicleType = await _unitOfWork.VehicleTypeRepository.Query().Where(x => x.VehicleTypeId.ToString() == vehicle.VehicleTypeId.ToString()).FirstOrDefaultAsync(); // lấy thông tin loại xe

                    if (distanceBetween <= DISTANCES[i] && vehicleType.Seats == model.Seats)
                    {
                        DataHubModel searchingDriver = new DataHubModel()
                        {
                            Id = driverId,
                            FirstName = item.Value.FirstName,
                            LastName = item.Value.LastName,
                            Phone = item.Value.Phone,
                            PhotoUrl = item.Value.PhotoUrl,
                            Gender = item.Value.Gender,
                            PlaceName = item.Value.PlaceName,
                            Address = item.Value.Address,
                            Longitude = lng,
                            Latitude = lat,
                            DistanceBetween = distanceBetween
                        };
                        _searchMapping.Add(model.Id, searchingDriver); // thêm những driver thỏa mãn điều kiện vào danh sách có thể phù hợp với yêu cầu từ khách hàng

                        _logger.LogInformation($"---------- Add success _searchMapping ---------------");
                    }
                }

                var driversList = _searchMapping.GetValues(model.Id); // lấy danh sách driver có thể phù hợp với khách hàng
                if (driversList == null || driversList.Count == 0)
                {
                    continue;
                }

                _logger.LogInformation("----------- FOUND THE DRIVERS LIST CAN BE MATCH WITH CUSTOMER -------------");

                // Lặp qua danh sách tài xế khả dụng để lấy điểm số
                foreach (var driverItem in driversList)
                {
                    // Lấy danh sách điểm đánh giá của driver
                    var driverRates = await _unitOfWork.FeedbackForDriverRepository
                                    .Query()
                                    .Where(x => x.DriverId.ToString() == driverItem.Id)
                                    .Select(x => x.Rate)
                                    .ToListAsync();
                    var driverRateAvg = driverRates.Count == 0 ? 0 : driverRates.Average(); // tính điểm trung bình rate của driver

                    // lây thông tin partner
                    var partnerId = (await _unitOfWork.DriverRepository.GetById(Guid.Parse(driverItem.Id))).PartnerId;

                    // lấy danh sách toàn bộ cuốc của dịch vụ đặt xe trong tháng
                    var allTripsInMonth = (await _unitOfWork.CustomerTripRepository
                                                    .Query()
                                                    .Where(CompareTime(Time.Day))
                                                    .Join(_unitOfWork.VehicleRepository.Query(),
                                                        customerTrip => customerTrip.VehicleId,
                                                        vehicle => vehicle.VehicleId,
                                                        (customerTrip, vehicle) => new { customerTrip, vehicle }
                                                    )
                                                    .Join(_unitOfWork.DriverRepository.Query(),
                                                        cusTripVeh => cusTripVeh.vehicle.VehicleId,
                                                        driver => driver.VehicleId,
                                                        (cusTripVeh, driver) => new { cusTripVeh, driver }
                                                    )
                                                    .Join(_unitOfWork.ServiceTypeRepository.Query(),
                                                        cusTripVehDri => cusTripVehDri.cusTripVeh.vehicle.ServiceTypeId,
                                                        serviceType => serviceType.ServiceTypeId,
                                                        (x, y) => new
                                                        {
                                                            DriverId = x.driver.DriverId,
                                                            VehicleId = x.cusTripVeh.vehicle.VehicleId,
                                                            PartnerId = x.driver.PartnerId,
                                                            ServiceTypeId = y.ServiceTypeId,
                                                            CreatedDate = x.cusTripVeh.customerTrip.CreatedDate
                                                        }
                                                    )
                                                    .Where(x => x.ServiceTypeId.ToString() == ServiceTypeDefaultData.BOOK_SERVICE_ID)
                                                    .Where(x => x.PartnerId == partnerId)
                                                    .ToListAsync());

                    // tính số cuốc partner trong tháng hiện tại
                    int amountTripsOfPartner = allTripsInMonth.Count;

                    // Tính số cuốc của driver trong ngày
                    int amountTripsOfDriver = 0;
                    foreach (var driverTripItem in allTripsInMonth)
                    {
                        if (driverTripItem.DriverId.ToString() == driverItem.Id && CompareTime(driverTripItem.CreatedDate, Time.Day))
                        {
                            amountTripsOfDriver += 1;
                        }
                    }


                    // tính điểm driver
                    driverItem.Point = 10 - (5.0 / (driverRateAvg == 0 ? 10 : driverRateAvg)) - (amountTripsOfDriver / 10) - (amountTripsOfPartner / 100);
                }



                // var rand = new Random();
                // var index = rand.Next(0, driversList.Count - 1);
                // var driver = driversList[index];

                driversList.Sort(delegate (DataHubModel x, DataHubModel y)
                {
                    return x.Point.CompareTo(y.Point);
                });
                _searchMapping.UpdateDictionaryValue(model.Id, driversList); // cập nhật lại danh sách tài xế có thể matching với yều cầu của khách hàng


                if (driversList.Count == 0)
                {
                    return;
                }

                var driver = driversList[0];
                driver.Status = (int)DriverStatus.OnBusy;
                _dataMapping.Add(driver.Id, driver, User.Driver);
                var driverResponse = await _driverService.UpdateDriverStatus(driver.Id, (int)DriverStatus.OnBusy); // cập nhật status driver xuống db
                if (driverResponse.StatusCode != 201)
                {
                    _logger.LogError("------------ ON OPEN DRIVER - Update Driver Status Failed to Database -----------");
                }

                isFoundDriver = true;
                // driver.IntervalLoopIndex = i;

                foreach (var cId in _connections.GetConnections(driver.Id))
                {
                    await Clients.Client(cId).SendAsync("BookingRequest", new
                    {
                        StatusCode = 200,
                        Driver = driver,
                        Customer = customer,
                        Type = "Booking",
                        Message = "Bạn nhận được yêu cầu đặt xe từ khách hàng."
                    });

                    _logger.LogInformation("----------- Already sent booking request to driver -------------");
                }

                break; // đã tìm thấy thì không tìm ở những khoảng cách xa hơn
            }

            if (!isFoundDriver)
            {
                // write code not found driver
                _logger.LogInformation("------------- DRIVER MATCHING WITH REQUEST NOT FOUND -------------");

                _searchMapping.Remove(customer.Id);

                customer.Status = (int)CustomerStatus.NotFound; // customer ko tìm thấy driver và không tìm lại
                _dataMapping.Add(customer.Id, customer, User.Customer);

                foreach (var connectionId in _connections.GetConnections(model.Id))
                {
                    await Clients.Client(connectionId).SendAsync("BookingResponse", new
                    {
                        StatusCode = 404,
                        Customer = "",
                        Type = "Booking",
                        Driver = "",
                        Message = "Không tìm thấy tài xế"
                    });
                }

            }

            _logger.LogInformation("---------------- End FindDriver Hub Function ------------------");
        }

        public string GetDriversListMatching(string json)
        {
            _logger.LogInformation("------------ Begin GetDriversListMatching Hub Function ------------");

            StartLocationBookingModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<StartLocationBookingModel>(json);

            if (model == null)
            {
                _logger.LogInformation("------------ EMPTY DRIVERS LIST ------------");
                _logger.LogInformation("------------ End GetDriversListMatching Hub Function ------------");
                return "[]";
            }

            List<object> list = null;
            foreach (var item in _dataMapping.GetDrivers(DriverStatus.On).GetItems())
            {

                if (list == null)
                {
                    list = new List<object>();
                }
                var driverId = item.Key;
                var lng = (double)item.Value.Longitude;
                var lat = (double)item.Value.Latitude;
                GeoCoordinate customerCoordinates = new GeoCoordinate((double)model.Latitude, (double)model.Longitude);
                GeoCoordinate driverCoordinates = new GeoCoordinate(lat, lng);
                double distanceBetween = customerCoordinates.GetDistanceTo(driverCoordinates); // tính khoảng cách giữa driver và customer

                if (distanceBetween <= DISTANCES[DISTANCES.Count - 1])
                {
                    list.Add(new
                    {
                        Longitude = lng,
                        Latitude = lat,
                    });
                }
            }

            if (list == null)
            {
                _logger.LogInformation($"------------ NOT FOUND DRIVERS ------------");
                return "[]";
            }

            _logger.LogInformation($"------------ Drivers List Matching With Customer ::::: Count: {list.Count} ------------");
            var parseToJson = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            _logger.LogInformation("------------ End GetDriversListMatching Hub Function ------------");

            return parseToJson;
        }

        public async Task<bool> CheckAcceptedRequest(string json, string status)
        {
            _logger.LogInformation("---------------- Begin CheckAcceptedRequest Hub Function ------------------");
            _logger.LogInformation($"---------------- JSON LOG {json} ------------------");

            DriverResponseModel response = Newtonsoft.Json.JsonConvert.DeserializeObject<DriverResponseModel>(json);

            _logger.LogInformation($"========================== STATUS ================== {status}");

            // Tài xế nhận yều cầu đặt xe
            if (status == "1")
            {
                _logger.LogInformation("---------------- Driver Accepted The Booking Request ------------------");

                // Lấy thông tin phương tiện thông qua driver
                var driver = await _unitOfWork.DriverRepository.GetById(Guid.Parse(response.Driver.Id));
                var vehicle = _vehicleStore.VehiclesList.GetValueOrDefault(driver.VehicleId.Value.ToString());

                // Tạo order cho yêu cầu đặt xe này
                OrderDetailsInfo orderDetails = new OrderDetailsInfo()
                {
                    Content = ServiceTypeDefaultData.BUS_SERVICE_CONTENT,
                    Price = response.Customer.Price,
                    Quantity = 1,
                    PriceOfBookingServiceId = Guid.Parse(response.Customer.PriceBookingId)
                };
                var orderDetailList = new List<OrderDetailsInfo>();
                orderDetailList.Add(orderDetails);
                CreateOrderModel createOrder = new CreateOrderModel()
                {
                    CustomerId = Guid.Parse(response.Customer.Id),
                    PartnerId = vehicle.PartnerId,
                    ServiceTypeId = Guid.Parse(ServiceTypeDefaultData.BOOK_SERVICE_ID), // maybe can use vehicle.ServiceTypeId
                    TotalPrice = orderDetails.Price,
                    OrderDetailsInfos = orderDetailList,
                    Distance = response.Customer.Distance
                };

                var respone = await _orderHelperService.CreateOrder(createOrder);
                if (respone.StatusCode != 201)
                {
                    return false;
                }

                var customerTrip = new CustomerTrip()
                {
                    CustomerTripId = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    CustomerId = Guid.Parse(response.Customer.Id),
                    VehicleId = vehicle.Id,
                    Distance = response.Customer.Distance,
                    Status = (int)CustomerTripStatus.Accepted
                };

                await _unitOfWork.CustomerTripRepository.Add(customerTrip);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("---------------- Make order and customer trip success !!!! ------------------");

                // cập nhật trạng thái đang đi đến đón khách hàng của driver
                var driverUpdate = _dataMapping.GetValue(response.Driver.Id, User.Driver);
                if (driverUpdate != null)
                {
                    _logger.LogInformation("---------- Update status Driver to On ARRIVING ---------");
                    driverUpdate.Status = (int)DriverStatus.OnArriving;
                    _dataMapping.Add(driverUpdate.Id, driverUpdate, User.Driver);
                }

                // cập nhật trạng thành đang đợi tài xế đến
                var customer = _dataMapping.GetValue(response.Customer.Id, User.Customer);
                if (customer != null)
                {
                    _logger.LogInformation("---------- Update status Customer to On WATTING ---------");
                    customer.Status = (int)CustomerStatus.Waiting;
                    _dataMapping.Add(customer.Id, customer, User.Customer);
                }

                // Gửi thông báo đến cho khách hàng 
                foreach (var connectionId in _connections.GetConnections(response.Customer.Id))
                {
                    await Clients.Client(connectionId).SendAsync("BookingResponse", new
                    {
                        StatusCode = 200,
                        Driver = response.Driver,
                        Message = "Yêu cầu đặt xe của bạn đã thành công"
                    });
                }

                // khách hàng đã đặt dc xe, reset lại d sách decline driver
                _driverDeclineList.RemoveAllValues(response.Customer.Id);

                // Lưu lại thông tin giữa khách hàng và tài xế khi đã giao tiếp với nhau
                _roomMapping.Add(response.Customer.Id, response.Driver.Id);
                _roomMapping.Add(response.Driver.Id, response.Customer.Id);

                return true;
            }
            else // Tài xế từ chối yêu cầu 
            {
                // lưu thông tin driver từ chối yêu cầu từ customer nào
                _driverDeclineList.Add(response.Customer.Id, response.Driver.Id);

                response.Driver.Status = (int)DriverStatus.On;
                _dataMapping.Add(response.Driver.Id, response.Driver, User.Driver);

                var driverResponse = await _driverService.UpdateDriverStatus(response.Driver.Id, (int)DriverStatus.On); // cập nhật status driver xuống db
                if (driverResponse.StatusCode != 201)
                {
                    _logger.LogError("------------ ON OPEN DRIVER - Update Driver Status Failed to Database -----------");
                }

                _logger.LogInformation("---------------- Driver Rejected The Booking Request ------------------");

                // Xóa tài xế ra khỏi danh sách matching
                var driversList = _searchMapping.GetValues(response.Customer.Id);
                driversList.RemoveAt(0);

                if (driversList.Count == 0) // không tìm thấy driver trong list có thể matching với customer thì duyệt ở độ dài khác
                {
                    _logger.LogInformation("---------------- Call FindDriver Function to extend distance range ------------------");

                    if (response.Customer.IntervalLoopIndex < DISTANCES.Count && response.Customer.Status == (int)CustomerStatus.Normal)
                    {
                        DataHubModel customer = _dataMapping.GetValue(response.Customer.Id, User.Customer);
                        if (customer.IntervalLoopIndex >= DISTANCES.Count - 1)
                        {
                            customer.IntervalLoopIndex = 0;
                        }
                        else
                        {
                            customer.IntervalLoopIndex += 1;
                        }

                        _dataMapping.Add(response.Customer.Id, customer, User.Customer); // cập nhật lại customer trong danh sách _datamapping

                        // xóa driver khỏi danh sách nhưng tài xế phù hợp với yêu cầu của khách hàng
                        // driversList.RemoveAt(response.Driver.ItemIndex);
                        // _searchMapping.UpdateDictionaryValue(response.Customer.Id, driversList); // cập nhật lại danh sách tài xế có thể matching với yều cầu của khách hàng

                        LocationModel model = new LocationModel();
                        model.Id = customer.Id;
                        model.Longitude = customer.Longitude;
                        model.Latitude = customer.Latitude;
                        model.Seats = customer.Seats;
                        model.Distance = customer.Distance;
                        model.Price = customer.Price;
                        model.PriceBookingId = customer.PriceBookingId;

                        string jsonLocationModel = Newtonsoft.Json.JsonConvert.SerializeObject(model); // parse object to json
                        await FindDriver(jsonLocationModel, "false");
                    }
                    else
                    {
                        _logger.LogInformation("-------------- DRIVER NOT FOUND -----------------");
                        foreach (var connectionId in _connections.GetConnections(response.Customer.Id))
                        {
                            await Clients.Client(connectionId).SendAsync("BookingResponse", new
                            {
                                StatusCode = 404,
                                Message = "Không tìm thấy tài xế"
                            });
                        }

                    }
                }
                else
                {
                    _logger.LogInformation("---------------- FOUNDED OTHER DRIVER ------------------");
                    var driver = driversList[0];
                    driver.Status = (int)DriverStatus.OnBusy;
                    _dataMapping.Add(driver.Id, driver, User.Driver);
                    var drp = await _driverService.UpdateDriverStatus(driver.Id, (int)DriverStatus.OnBusy); // cập nhật status driver xuống db
                    if (drp.StatusCode != 201)
                    {
                        _logger.LogError("------------ ON OPEN DRIVER - Update Driver Status Failed to Database -----------");
                    }

                    foreach (var cId in _connections.GetConnections(driver.Id))
                    {
                        await Clients.Client(cId).SendAsync("BookingRequest", new
                        {
                            StatusCode = 200,
                            Driver = driver,
                            Customer = response.Customer,
                            Type = "Booking",
                            Message = "Bạn nhận được yêu cầu đặt xe từ khách hàng."
                        });

                        _logger.LogInformation("----------- Already sent booking request to driver -------------");
                    }
                }
            }

            _logger.LogInformation("---------------- End CheckAcceptedRequest Hub Function ------------------");
            return true;
        }

        public async Task<bool> CancelBooking(string customerId)
        {
            _logger.LogInformation("---------------- Customer cancelled booking request ------------------");

            var driverId = _roomMapping.GetValue(customerId);
            var customer = _dataMapping.GetValue(customerId, User.Customer);
            var driver = _dataMapping.GetValue(driverId, User.Driver);
            if (customer != null && driver != null)
            {
                GeoCoordinate customerCoordinates = new GeoCoordinate(customer.Latitude, customer.Longitude);
                GeoCoordinate driverCoordinates = new GeoCoordinate(driver.Latitude, driver.Longitude);
                double distanceBetween = customerCoordinates.GetDistanceTo(driverCoordinates); // tính khoảng cách giữa driver và customer 

                _logger.LogInformation($"----------- KHOẢNG CÁCH HIỆN TẠI GIỮA CUSTOMER AND DRIVER:  {distanceBetween}-----------------");
                _logger.LogInformation($"----------- KHOẢNG CÁCH LÚC ĐẦU GIỮA CUSTOMER AND DRIVER:  {driver.DistanceBetween}-----------------");

                var refundPrice = 0M;
                if (driver.DistanceBetween > 2000 && ((double)driver.DistanceBetween - distanceBetween) >= ((double)driver.DistanceBetween * 0.6))
                {
                    _logger.LogInformation("---------------- Refund a part total price of order  ------------------");
                    refundPrice = customer.Price - customer.Price * 0.3M; // tính phí vi phạm
                }
                else
                {
                    _logger.LogInformation("---------------- Refund all total price of order  ------------------");
                    refundPrice = customer.Price; // trả lại toàn bộ tiền cho khách
                }

                var order = await _unitOfWork.OrderRepository
                            .Query()
                            .Where(x => x.ServiceTypeId.ToString() == ServiceTypeDefaultData.BOOK_SERVICE_ID)
                            .Where(x => x.CustomerId.ToString() == customer.Id)
                            .Where(x => x.Status != (int)OrderStatus.Canceled && x.Status != (int)OrderStatus.Done)
                            .OrderByDescending(x => x.CreatedDate)
                            .FirstOrDefaultAsync();

                // cập nhật lại giá tiền và trạng thái của order
                order.TotalPrice = order.TotalPrice - refundPrice;
                order.Status = (int)OrderStatus.Canceled;
                _unitOfWork.OrderRepository.Update(order);

                // cập nhật lại trạng thái của customertrip
                var customerTrip = await _unitOfWork.CustomerTripRepository
                                    .Query()
                                    .Where(x => x.CustomerId.ToString() == customer.Id)
                                    .Where(x => x.Status == (int)CustomerTripStatus.Accepted)
                                    .FirstOrDefaultAsync();

                customerTrip.Status = (int)CustomerTripStatus.Canceled;
                _unitOfWork.CustomerTripRepository.Update(customerTrip);

                // Tạo transaction cho customer
                var wallet = await _unitOfWork.WalletRepository.Query().Where(x => x.CustomerId.ToString().Equals(customer.Id)).FirstOrDefaultAsync();
                var transaction = new Transaction()
                {
                    TransactionId = Guid.NewGuid(),
                    WalletId = wallet.WalletId,
                    Amount = refundPrice,
                    Content = "Hoàn trả tiền cho khách hàng",
                    CreatedDate = DateTime.Now,
                    OrderId = order.OrderId,
                    Status = 1,
                };
                wallet.AccountBalance += refundPrice;
                _unitOfWork.WalletRepository.Update(wallet);
                await _unitOfWork.TransactionRepository.Add(transaction);

                // Lấy ví của partner
                var partnerWallet = await _unitOfWork.WalletRepository
                                        .Query()
                                        .Where(x => x.PartnerId == order.PartnerId)
                                        .FirstOrDefaultAsync();

                // Tạo transaction cho partner
                var partnerTransaction = new Transaction()
                {
                    TransactionId = Guid.NewGuid(),
                    Content = $"Đối tác gửi lại 90% tiền thừa",
                    OrderId = order.OrderId,
                    CreatedDate = DateTime.Now,
                    Amount = -(refundPrice * 0.9M), // xét dấu âm cho giao dịch trừ tiền
                    Status = 1,
                    WalletId = partnerWallet.WalletId
                };
                partnerWallet.AccountBalance = partnerWallet.AccountBalance - (-partnerTransaction.Amount); // trừ tiền từ transaction
                await _unitOfWork.TransactionRepository.Add(partnerTransaction);
                _unitOfWork.WalletRepository.Update(partnerWallet);

                // Tạo transaction cho admin
                var adminWallet = await _unitOfWork.WalletRepository
                                    .Query()
                                    .Where(x => x.PartnerId == null && x.CustomerId == null)
                                    .FirstOrDefaultAsync();

                var adminTransaction = new Transaction()
                {
                    TransactionId = Guid.NewGuid(),
                    Content = $"Hệ Thống Gửi lại 10% tiền thừa",
                    OrderId = order.OrderId,
                    CreatedDate = DateTime.Now,
                    Amount = -(refundPrice * 0.1M), // xét dấu âm cho giao dịch trừ tiền
                    Status = 1,
                    WalletId = adminWallet.WalletId
                };
                adminWallet.AccountBalance = adminWallet.AccountBalance - (-adminTransaction.Amount); // trừ tiền từ transaction
                await _unitOfWork.TransactionRepository.Add(adminTransaction);
                _unitOfWork.WalletRepository.Update(adminWallet);

                await _unitOfWork.SaveChangesAsync();

                await _bookingService.RefundBooking((double)refundPrice, Guid.Parse(customerId));

                // cập nhật status customer
                customer.Status = (int)CustomerStatus.Normal;
                _dataMapping.Add(customerId, customer, User.Customer);

                // cập nhật status driver
                driver.Status = (int)DriverStatus.On;
                _dataMapping.Add(driverId, driver, User.Driver);

                // xóa d sách driver matching với customer
                _searchMapping.Remove(customerId);

                // xóa customer and driver khỏi danh sách kết nối (room)
                _roomMapping.Remove(driverId);
                _roomMapping.Remove(customerId);


                //Thông báo tới driver là khách hàng đã hủy booking

                foreach (var connectionId in _connections.GetConnections(driverId))
                {
                    await Clients.Client(connectionId).SendAsync("CanceledBooking", new
                    {
                        StatusCode = 200,
                        Message = "Khách hàng đã hủy cuốc xe"
                    });
                }

                _logger.LogInformation("---------------- End cancel booking hub function  ------------------");

                return true;
            }

            return false;
        }

        public async Task DriverArrived(string driverId)
        {
            _logger.LogInformation("---------------- BEGIN Driver Arrived Hub Function  ------------------");

            var customerId = _roomMapping.GetValue(driverId);
            foreach (var connectionId in _connections.GetConnections(customerId))
            {
                await Clients.Client(connectionId).SendAsync("DriverArrived", new
                {
                    StatusCode = 200,
                    Message = "Tài xế đã đến điểm đón"
                });
            }

            // cập nhật trạng thái của driver sau khi đã đến điểm đón
            var driver = _dataMapping.GetValue(driverId, User.Driver);
            if (driver != null)
            {
                driver.Status = (int)DriverStatus.OnArrived;
                _dataMapping.Add(driver.Id, driver, User.Driver);
            }

            // cập nhật trạng thái của driver sau khi đã đến điểm đón
            var customer = _dataMapping.GetValue(customerId, User.Customer);
            if (customer != null)
            {
                customer.Status = (int)CustomerStatus.DriverArrived;
                _dataMapping.Add(customerId, customer, User.Customer);
            }

            _logger.LogInformation("---------------- END Driver Arrived Hub Function  ------------------");

        }

        public async Task DriverPickedUp(string driverId)
        {
            _logger.LogInformation("---------------- BEGIN Driver Picked Up Hub Function  ------------------");

            var customerId = _roomMapping.GetValue(driverId);
            var customerTrip = await _unitOfWork.CustomerTripRepository
                                    .Query()
                                    .Where(x => x.CustomerId.ToString() == customerId)
                                    .Where(x => x.Status == (int)CustomerTripStatus.Accepted)
                                    .OrderByDescending(x => x.CreatedDate)
                                    .FirstOrDefaultAsync();

            customerTrip.Status = (int)CustomerTripStatus.PickedUp;
            _unitOfWork.CustomerTripRepository.Update(customerTrip);
            await _unitOfWork.SaveChangesAsync();

            foreach (var connectionId in _connections.GetConnections(customerId))
            {
                await Clients.Client(connectionId).SendAsync("DriverPickedUp", new
                {
                    StatusCode = 200,
                    Message = "Bạn đã lên xe!"
                });
            }

            // cập nhật trạng thái của customer
            var customerDataHubModel = _dataMapping.GetValue(customerId, User.Customer);
            if (customerDataHubModel != null)
            {
                customerDataHubModel.Status = (int)CustomerStatus.PickedUp;
                _dataMapping.Add(customerId, customerDataHubModel, User.Customer);
            }

            // cập nhật trạng thái của driver
            var driverDataHubModel = _dataMapping.GetValue(driverId, User.Driver);
            if (driverDataHubModel != null)
            {
                driverDataHubModel.Status = (int)DriverStatus.PickedUp;
                _dataMapping.Add(driverId, driverDataHubModel, User.Driver);
            }

            _logger.LogInformation("---------------- END Driver Picked Up Hub Function  ------------------");
        }

        public async Task CompletedBooking(string driverId)
        {
            _logger.LogInformation("---------------- BEGIN Completed Booking Hub Function  ------------------");

            var customerId = _roomMapping.GetValue(driverId);
            var customerTrip = await _unitOfWork.CustomerTripRepository
                                    .Query()
                                    .Where(x => x.CustomerId.ToString() == customerId)
                                    .Where(x => x.Status == (int)CustomerTripStatus.PickedUp)
                                    .OrderByDescending(x => x.CreatedDate)
                                    .FirstOrDefaultAsync();

            customerTrip.Status = (int)CustomerTripStatus.Done;
            _unitOfWork.CustomerTripRepository.Update(customerTrip);

            var order = await _unitOfWork.OrderRepository
                            .Query()
                            .Where(x => x.ServiceTypeId.ToString() == ServiceTypeDefaultData.BOOK_SERVICE_ID)
                            .Where(x => x.CustomerId.ToString() == customerId)
                            .Where(x => x.Status == (int)OrderStatus.Paid)
                            .OrderByDescending(x => x.CreatedDate)
                            .FirstOrDefaultAsync();

            // cập nhật lại giá tiền và trạng thái của order
            order.Status = (int)OrderStatus.Done;
            _unitOfWork.OrderRepository.Update(order);

            await _unitOfWork.SaveChangesAsync();

            foreach (var connectionId in _connections.GetConnections(customerId))
            {
                await Clients.Client(connectionId).SendAsync("CompletedBooking", new
                {
                    StatusCode = 200,
                    Message = "Cảm ơn quý khách đã đặt xe."
                });
            }

            // xóa customer and driver khỏi danh sách kết nối (room)
            _roomMapping.Remove(driverId);
            _roomMapping.Remove(customerId);

            // update data mapping list
            var driverDataHubModel = _dataMapping.GetValue(driverId, User.Driver);
            driverDataHubModel.Status = (int)DriverStatus.On;
            _dataMapping.Add(driverId, driverDataHubModel, User.Driver);

            var customerDataHubModel = _dataMapping.GetValue(customerId, User.Customer);
            customerDataHubModel.Status = (int)CustomerStatus.Normal;
            _dataMapping.Add(customerId, customerDataHubModel, User.Customer);

            // Xóa khỏi danh sách tìm kiếm 
            _searchMapping.Remove(customerId);

            _logger.LogInformation("---------------- BEGIN Completed Booking Hub Function  ------------------");
        }

        public async Task CanceledFinding(string customerId)
        {
            _logger.LogInformation("-------------------------------------- BEGIN Canceled Finding Driver Hub Function  ----------------------------------------------------");

            // cập nhật lại trạng thái ban đầu của customer
            var customerDataHubModel = _dataMapping.GetValue(customerId, User.Customer);
            if (customerDataHubModel != null)
            {
                customerDataHubModel.Status = (int)CustomerStatus.Normal;
                customerDataHubModel.IntervalLoopIndex = 0;
                _dataMapping.Add(customerId, customerDataHubModel, User.Customer);
            }

            //
            var tempdata = _dataMapping.GetValue(customerId, User.Customer);

            var driversList = _searchMapping.GetValues(customerId); // lấy danh sách driver có thể phù hợp với khách hàng
            _logger.LogInformation($"---------------  SỐ LƯỢNG DRIVERS TRONG SEARCHING LIST WITH CUSTOMER::: {driversList.Count} --------------");
            if (driversList != null && driversList.Count > 0)
            {
                foreach (var driverItem in driversList)
                {
                    // reset lại toàn bộ status của driver về ban đầu khi dc assign vào list matching với customer
                    var driverDataHubModel = _dataMapping.GetValue(driverItem.Id, User.Driver);
                    if (driverDataHubModel != null)
                    {
                        driverDataHubModel.Status = (int)DriverStatus.On;
                        _dataMapping.Add(driverItem.Id, driverDataHubModel, User.Driver);
                        var driverResponse = await _driverService.UpdateDriverStatus(driverDataHubModel.Id, (int)DriverStatus.On); // cập nhật status driver xuống db
                        if (driverResponse.StatusCode != 201)
                        {
                            _logger.LogError("------------ ON OPEN DRIVER - Update Driver Status Failed to Database -----------");
                        }
                    }

                    foreach (var connectionId in _connections.GetConnections(driverItem.Id))
                    {
                        Task.WaitAll(Clients.Client(connectionId).SendAsync("FindingOut", new
                        {
                            StatusCode = 210,
                            Message = "Khách hàng đã thoát ứng dụng"
                        }));
                    }

                    _driverDeclineList.Remove(driverItem.Id);
                }

                _searchMapping.Remove(customerId); // reset lại danh sách matching giữa customer và driver

            }
            _logger.LogInformation("---------------- END Canceled Finding Driver Hub Function  ------------------");
            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        public async Task<bool> OpenDriver(string json) // hàm được gọi khi tài xế bật chế độ nhận yêu cầu đặt xe từ khách hàng
        {
            _logger.LogInformation("------------ Begin OpenDriver Hub Function -----------");
            _logger.LogInformation($"------------ Begin OpenDriver Hub Function JSON::: {json} -----------");

            LocationModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<LocationModel>(json);
            var dataHubModel = _dataMapping.GetValue(model.Id, User.Driver);
            var driverStatus = dataHubModel.Status;
            if (dataHubModel != null)
            {
                dataHubModel.Longitude = model.Longitude;
                dataHubModel.Latitude = model.Latitude;
                // dataHubModel.Status = dataHubModel.Status == (int)DriverStatus.On
                //                                             ? dataHubModel.Status
                //                                             : dataHubModel.Status != (int)DriverStatus.OnBusy
                //                                                     ? (int)DriverStatus.OnBusy
                //                                                     : dataHubModel.Status;
                if (dataHubModel.Status == (int)DriverStatus.Off || dataHubModel.Status == (int)DriverStatus.Active)
                {
                    dataHubModel.Status = (int)DriverStatus.On;
                }

                _dataMapping.Add(model.Id, dataHubModel, User.Driver);

                var drivervalue = _dataMapping.GetValue(model.Id, User.Driver);
                var drivervaluejson = Newtonsoft.Json.JsonConvert.SerializeObject(drivervalue);
                _logger.LogInformation($"------------ Update status driver in DATA MAPPING :::: JSON: {drivervaluejson}  -------------");
            }

            if (driverStatus == (int)DriverStatus.Off || driverStatus == (int)DriverStatus.Active)
            {
                var driverResponse = await _driverService.UpdateDriverStatus(model.Id, (int)DriverStatus.On); // cập nhật status driver xuống db
                if (driverResponse.StatusCode != 201)
                {
                    _logger.LogError("------------ ON OPEN DRIVER - Update Driver Status Failed to Database -----------");
                    return false;
                }
            }

            _logger.LogInformation("------------ End OpenDriver Hub Function -----------");

            return true;
        }

        public async Task<bool> CloseDriver(string driverId) // hàm được gọi khi tài xế tắt chế độ nhận yêu cầu đặt xe từ khách hàng
        {
            _logger.LogInformation("------------ Begin CloseDriver Hub Function -----------");

            // _dataMapping.Remove(driverId, User.Driver);
            // cập nhật lại trạng thái driver về chế độ tắt nhận request
            var driverDataHubModel = _dataMapping.GetValue(driverId, User.Driver);
            if (driverDataHubModel == null)
            {
                _logger.LogInformation("------------ Driver Not Found to Close -----------");
                return false;
            }


            if (driverDataHubModel.Status != (int)DriverStatus.OnArrived &&
                    driverDataHubModel.Status != (int)DriverStatus.OnArriving &&
                    driverDataHubModel.Status != (int)DriverStatus.PickedUp
                )
            {
                driverDataHubModel.Status = (int)DriverStatus.Off;
                _dataMapping.Add(driverId, driverDataHubModel, User.Driver);
                var driverResponse = await _driverService.UpdateDriverStatus(driverId, (int)DriverStatus.Off);
                if (driverResponse.StatusCode != 201)
                {
                    _logger.LogError("------------ Update Driver Status Failed to Database -----------");
                    return false;
                }

                var customerId = _roomMapping.GetValue(driverId);
                if (driverId != null) _roomMapping.Remove(driverId);
                if (customerId != null) _roomMapping.Remove(customerId);
            }

            // var drivervalue = _dataMapping.GetValue(driverId, User.Driver);
            // var drivervaluejson = Newtonsoft.Json.JsonConvert.SerializeObject(drivervalue);

            _driverDeclineList.Remove(driverId);

            _logger.LogInformation("------------ End CloseDriver Hub Function -----------");
            return true;
        }

        private async Task LoadVehiclesList()
        {
            if (_vehicleStore.VehiclesList.Count > 0)
            {
                _vehicleStore.VehiclesList.Clear();
            }

            _vehicleStore.VehiclesList = await _unitOfWork.VehicleRepository
                            .Query()
                            .Where(x => x.ServiceTypeId.ToString() == ServiceTypeDefaultData.BOOK_SERVICE_ID)
                            .Select(x => x.AsVehicleViewModel())
                            .ToDictionaryAsync(x => x.Id.ToString());
        }

        public Task<string> GetDriverByCustomer(string customerId)
        {
            var driverId = _roomMapping.GetValue(customerId);
            if (driverId != null)
            {
                var driver = _dataMapping.GetValue(driverId, User.Driver);
                object obj = new
                {
                    Longitude = driver.Longitude,
                    Latitude = driver.Latitude,
                };

                var parseToJson = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                return Task.FromResult(parseToJson);
            }

            return Task.FromResult("{}");
        }


        public Task<string> GetDriverStatus(string driverId)
        {
            var driver = _dataMapping.GetValue(driverId, User.Driver);
            if (driver != null)
                return Task.FromResult(driver.Status.ToString());
            return Task.FromResult("");
        }

        public Task<string> GetCustomerStatus(string customerId)
        {
            var customer = _dataMapping.GetValue(customerId, User.Customer);
            if (customer != null)
                return Task.FromResult(customer.Status.ToString());
            return Task.FromResult("");
        }

        // ------------- Overrivde method --------------
        public override Task OnConnectedAsync() // hàm được gọi khi có thiết bị connect tới hub
        {
            _logger.LogInformation("------------------ Begin On Connected Async Hub Function --------------------");

            // load vehicle có dịch vụ là đặt xe
            if (_vehicleStore.VehiclesList.Count == 0)
            {
                Task.WaitAll(LoadVehiclesList());
                _logger.LogInformation("---------------- Load Vehicles List Hub Function Success --------------");
                _logger.LogInformation($"---------------- Vehicles List Hub Function Success - Count: {_vehicleStore.VehiclesList.Count} --------------");
            }

            // Xác định id người dùng
            bool isCustomerRole = true;
            dynamic id = Context.User.Claims.Where(x => x.Type == "CustomerId").Select(x => x.Value).FirstOrDefault();
            if (id == null)
            {
                var vehicleId = (Context.User.Claims.Where(x => x.Type == "VehicleId").Select(x => x.Value).FirstOrDefault())?.ToString();
                if (string.IsNullOrEmpty(vehicleId))
                {
                    _logger.LogInformation("------------ Driver has not assigned the vehicle--------------");
                    return Task.CompletedTask;
                }

                // kiểm tra loại dịch vụ có phải là đặt xe hay không
                if (!_vehicleStore.VehiclesList.ContainsKey(vehicleId))
                {
                    _logger.LogInformation("------------ Vehicle is not for booking service --------------");
                    return Task.CompletedTask;
                }


                _logger.LogInformation("------------ Driver Role BE ASSIGNED --------------");
                id = Context.User.Claims.Where(x => x.Type == "DriverId").Select(x => x.Value).FirstOrDefault();
                isCustomerRole = false;
            }

            _logger.LogInformation($"--------------- OnConnectedAsync Context.User.Claim: {id} - {Context.ConnectionId} -------------");

            _connections.Add(id, Context.ConnectionId); // lưu id người dùng và id máy người dùng (connectionId)

            if (isCustomerRole)
            {
                DataHubModel customerHubViewModel = new DataHubModel();
                foreach (var claim in Context.User.Claims.ToList())
                {
                    switch (claim.Type)
                    {
                        case "CustomerId":
                            {
                                customerHubViewModel.Id = claim.Value;
                                break;
                            }
                        case "FirstName":
                            {
                                customerHubViewModel.FirstName = claim.Value;
                                break;
                            }
                        case "LastName":
                            {
                                customerHubViewModel.LastName = claim.Value;
                                break;
                            }
                        case "Gender":
                            {
                                customerHubViewModel.Gender = claim.Value;
                                break;
                            }
                        case "Phone":
                            {
                                customerHubViewModel.Phone = claim.Value;
                                break;
                            }
                        case "PhotoUrl":
                            {
                                customerHubViewModel.PhotoUrl = claim.Value;
                                break;
                            }
                        case "Status":
                            {
                                customerHubViewModel.Status = (int)CustomerStatus.Normal;
                                break;
                            }
                        default: break;
                    }
                }

                // lấy thông tin trạng thái customer trước đó trước khi cập nhật
                var customerUpdated = _dataMapping.GetValue(customerHubViewModel.Id, User.Customer);
                if (customerUpdated != null &&
                    (customerUpdated.Status == (int)CustomerStatus.DriverArrived ||
                        customerUpdated.Status == (int)CustomerStatus.PickedUp ||
                        customerUpdated.Status == (int)CustomerStatus.Waiting
                    )
                )
                {
                    customerHubViewModel.Status = customerUpdated.Status;
                }

                _dataMapping.Add(customerHubViewModel.Id, customerHubViewModel, User.Customer);

                _logger.LogInformation($"--------------- Customer ON CONNECT -------------");
                _logger.LogInformation($"--------------- Customer ROLE: {customerHubViewModel.LastName} - {customerHubViewModel.FirstName} -------------");
            }
            else
            {
                DataHubModel driverHubViewModel = new DataHubModel();
                foreach (var claim in Context.User.Claims.ToList())
                {
                    switch (claim.Type)
                    {
                        case "DriverId":
                            {
                                driverHubViewModel.Id = claim.Value;
                                break;
                            }
                        case "FirstName":
                            {
                                driverHubViewModel.FirstName = claim.Value;
                                break;
                            }
                        case "LastName":
                            {
                                driverHubViewModel.LastName = claim.Value;
                                break;
                            }
                        case "Gender":
                            {
                                driverHubViewModel.Gender = claim.Value;
                                break;
                            }
                        case "Phone":
                            {
                                driverHubViewModel.Phone = claim.Value;
                                break;
                            }
                        case "PhotoUrl":
                            {
                                driverHubViewModel.PhotoUrl = claim.Value;
                                break;
                            }
                        case "Status":
                            {
                                driverHubViewModel.Status = (int)DriverStatus.Off; // Khi mới kết nối vô app thì tài xế mặc định tài xế đang không bật chế độ nhận yêu cầu booking
                                break;
                            }
                        default: break;
                    }
                }

                _logger.LogInformation($"--------------- DRIVER ON CONNECT -------------");
                _logger.LogInformation($"--------------- DRIVER ROLE: {driverHubViewModel.LastName} - {driverHubViewModel.FirstName} -------------");

                var driverUpdated = _dataMapping.GetValue(driverHubViewModel.Id, User.Driver);
                if (driverUpdated != null &&
                    (driverUpdated.Status == (int)DriverStatus.OnArriving ||
                        driverUpdated.Status == (int)DriverStatus.OnArrived ||
                        driverUpdated.Status == (int)DriverStatus.PickedUp
                    )
                )
                {
                    driverHubViewModel.Status = driverUpdated.Status;
                }

                // Chỉ thêm những tài xế đang có xe chạy dịch vụ đặt xe
                var driver = _unitOfWork.DriverRepository.GetById(Guid.Parse(driverHubViewModel.Id)).Result;
                if (driver.VehicleId != null && _vehicleStore.VehiclesList.ContainsKey(driver.VehicleId.Value.ToString()))
                {
                    _dataMapping.Add(driverHubViewModel.Id, driverHubViewModel, User.Driver);
                    Task.WaitAll(_driverService.UpdateDriverStatus(driver.DriverId.ToString(), (int)DriverStatus.Off)); // cập nhật trạng thái xuống database
                    _logger.LogInformation("------------------ Added Driver To DataMapping Store --------------------");
                    _logger.LogInformation($"------------------ Drivers List in DataMapping Store :::: Count: {_dataMapping.GetDrivers().Count} --------------------");
                }

                _logger.LogInformation($"Number of connections of ID: {driverHubViewModel.Id} - ConnectionCount: {_connections.GetConnectionsCount(driverHubViewModel.Id)}");
                _logger.LogInformation("------------------ End On Connected Async Hub Function --------------------");
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception) // hàm được gọi khi có thiết bị ngắt kết nối tới hub
        {
            _logger.LogInformation("------------------------Begin On Disconnected Async--------------------------");
            dynamic id = (Context.User.Claims.Where(x => x.Type == "CustomerId").Select(x => x.Value).FirstOrDefault())?.ToString();
            if (id == null)
            {
                id = (Context.User.Claims.Where(x => x.Type == "DriverId").Select(x => x.Value).FirstOrDefault()).ToString();
                var driverHubModel = _dataMapping.GetValue(id, User.Driver);
                if (driverHubModel.Status != (int)DriverStatus.OnArrived &&
                    driverHubModel.Status != (int)DriverStatus.OnArriving &&
                    driverHubModel.Status != (int)DriverStatus.PickedUp
                )
                {
                    driverHubModel.Status = (int)DriverStatus.Off;
                    Task.WaitAll(_driverService.UpdateDriverStatus(id, (int)DriverStatus.Off)); // cập nhật status driver xuống db
                }

                _dataMapping.Add(id, driverHubModel, User.Driver);

                _logger.LogInformation("------------------------ Driver is updated Status --------------------------");
            }
            else
            {
                var customerHubModel = _dataMapping.GetValue(id, User.Customer);
                if (customerHubModel != null &&
                    (customerHubModel.Status != (int)CustomerStatus.DriverArrived &&
                        customerHubModel.Status != (int)CustomerStatus.PickedUp &&
                        customerHubModel.Status != (int)CustomerStatus.Waiting &&
                        customerHubModel.Status != (int)CustomerStatus.Finding
                    )
                )
                {
                    customerHubModel.Status = (int)CustomerStatus.Normal;
                    _dataMapping.Add(id, customerHubModel, User.Customer);
                }
                else if (customerHubModel.Status == (int)CustomerStatus.Finding)
                {
                    customerHubModel.Status = (int)CustomerStatus.OnDisconnect;
                    _dataMapping.Add(id, customerHubModel, User.Customer);

                    var driversList = _searchMapping.GetValues(id);

                    foreach (var driverItem in driversList)
                    {
                        string driverId = $"{driverItem.Id}";
                        foreach (var connectionId in _connections.GetConnections(driverId))
                        {
                            Task.WaitAll(Clients.Client(connectionId).SendAsync("FindingOut", new
                            {
                                StatusCode = 200,
                                Message = "Khách hàng đã thoát ứng dụng"
                            }));
                        }
                    }
                }

            }

            _driverDeclineList.Remove(id);

            _logger.LogInformation($"OnDisconnectedAsync Context.User.Claim: {id} - {Context.ConnectionId}");

            _connections.Remove(id, Context.ConnectionId);

            _logger.LogInformation($"Number of connections of ID: {id} - ConnectionCount: {_connections.GetConnectionsCount(id)}");

            _logger.LogInformation("------------------------End On Disconnected Async--------------------------");

            return base.OnDisconnectedAsync(exception);
        }
    }
}