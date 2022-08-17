using GeoCoordinatePortable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Hubs.Mapping;
using TourismSmartTransportation.Business.Hubs.Models;
using TourismSmartTransportation.Business.Hubs.Store;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.Order;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.OrderDetail;
using TourismSmartTransportation.Business.ViewModel.Partner.VehicleManagement;
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

        // Global parameters to store data
        private readonly List<double> DISTANCES = new List<double>() { 3000, 5000, 7000, 10000 };
        private readonly static VehicleStore _vehicleStore = new VehicleStore();

        // Mapping data store
        private readonly static SearchMapping _seachMapping = new SearchMapping();
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>(); // Tạo instance connection mapping
        private readonly static DataMapping _dataMapping = new DataMapping(); // Tạo instance lưu danh sách thông tin customer và danh sách thông tin driver

        public NotificationHub(ILogger<NotificationHub> logger,
                                IUnitOfWork unitOfWork,
                                IDriverManagementService driverService,
                                IOrderHelpersService orderHelpersService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _driverService = driverService;
            _orderHelperService = orderHelpersService;
        }

        public async Task FindDriver(string json)
        {
            _logger.LogInformation("---------------- Begin FindDriver Hub Function ------------------");

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
                _dataMapping.Add(model.Id, customer, User.Customer);
            }

            bool isFoundDriver = false;

            for (int i = customer.IntervalLoopIndex; i < DISTANCES.Count; i++) // Tìm kiếm trong những khoảng distance 3000m 5000m 7000m
            {
                _logger.LogInformation($"---------- Go to IntervalLoopIndex - {i}---------------");

                foreach (var item in _dataMapping.GetDrivers(DriverStatus.On).GetItems())
                {
                    _logger.LogInformation($"---------- GO TO FIND MATCHING DRIVER ---------------");
                    var driverId = item.Key;
                    var lng = (double)item.Value.Longitude;
                    var lat = (double)item.Value.Latitude;
                    GeoCoordinate customerCoordinates = new GeoCoordinate((double)model.Latitude, (double)model.Longitude);
                    GeoCoordinate driverCoordinates = new GeoCoordinate(lat, lng);
                    double distanceBetween = customerCoordinates.GetDistanceTo(driverCoordinates); // tính khoảng cách giữa driver và customer

                    var d = await _unitOfWork.DriverRepository.GetById(Guid.Parse(driverId)); // lấy thông tin driver
                    var vehicle = _vehicleStore.VehiclesList.GetValueOrDefault(d.VehicleId.Value.ToString()); // lấy thông tin xe mà driver đang được cấp
                    var vehicleType = await _unitOfWork.VehicleTypeRepository.Query().Where(x => x.VehicleTypeId == vehicle.VehicleTypeId).FirstOrDefaultAsync(); // lấy thông tin loại xe

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
                        _seachMapping.Add(model.Id, searchingDriver); // thêm những driver thỏa mãn điều kiện vào danh sách có thể phù hợp với yêu cầu từ khách hàng
                    }
                }

                var driversList = _seachMapping.GetValues(model.Id); // lấy danh sách driver có thể phù hợp với khách hàng
                if (driversList == null)
                {
                    continue;
                }

                _logger.LogInformation("----------- DRIVER FOUNDED -------------");
                // implement thuật toán tìm tài xế
                var rand = new Random();
                var index = rand.Next(0, driversList.Count - 1);
                var driver = driversList[index];
                driver.ItemIndex = index;
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
            }

            _logger.LogInformation("---------------- End FindDriver Hub Function ------------------");
        }

        public List<object> GetDriversListMatching(string json)
        {
            _logger.LogInformation("------------ Begin GetDriversListMatching Hub Function ------------");

            StartLocationBookingModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<StartLocationBookingModel>(json);
            List<object> list = null;
            var abcList = _dataMapping.GetDrivers(DriverStatus.On).GetItems().Count;
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

            if (list != null)
            {
                _logger.LogInformation($"------------ Drivers List Matching With Customer ::::: Count: {list.Count} ------------");
            }
            _logger.LogInformation("------------ End GetDriversListMatching Hub Function ------------");

            return list;
        }

        public async Task CheckAcceptedRequest(string json)
        {
            _logger.LogInformation("---------------- Begin CheckAcceptedRequest Hub Function ------------------");

            DriverResponseModel response = Newtonsoft.Json.JsonConvert.DeserializeObject<DriverResponseModel>(json);
            // Tài xế nhận yều cầu đặt xe
            if (response.Accepted)
            {
                _logger.LogInformation("---------------- Driver Accepted The Booking Request ------------------");

                foreach (var connectionId in _connections.GetConnections(response.Customer.Id))
                {
                    await Clients.Client(connectionId).SendAsync("BookingResponse", new
                    {
                        StatusCode = 200,
                        Driver = response.Driver,
                        Message = "Yêu cầu đặt xe của bạn đã thành công"
                    });
                }

                // Lấy thông tin phương tiện thông qua driver
                var driver = await _unitOfWork.DriverRepository.GetById(Guid.Parse(response.Driver.Id));
                var vehicle = _vehicleStore.VehiclesList.GetValueOrDefault(driver.VehicleId.Value.ToString());

                // Tạo order cho yêu cầu đặt xe này
                OrderDetailsInfo orderDetails = new OrderDetailsInfo()
                {
                    Content = ServiceTypeDefaultData.BUS_SERVICE_CONTENT,
                    Price = response.Customer.Price,
                    Quantity = 1,
                    PriceOfBusServiceId = Guid.Parse(response.Customer.PriceBookingId)
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
                    // thông báo lỗi nếu cần
                }

                var customerTrip = new CustomerTrip()
                {
                    CustomerTripId = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    CustomerId = Guid.Parse(response.Customer.Id),
                    VehicleId = vehicle.VehicleTypeId,
                    Distance = response.Customer.Distance,
                    Status = (int)CustomerTripStatus.Accepted
                };
                await _unitOfWork.CustomerTripRepository.Add(customerTrip);
                await _unitOfWork.SaveChangesAsync();
            }
            else // Tài xế từ chối yêu cầu 
            {
                _logger.LogInformation("---------------- Driver Rejected The Booking Request ------------------");

                if (response.Customer.IntervalLoopIndex < DISTANCES.Count)
                {
                    DataHubModel customer = _dataMapping.GetValue(response.Customer.Id, User.Customer);
                    customer.IntervalLoopIndex += 1;
                    _dataMapping.Add(response.Customer.Id, customer, User.Customer); // cập nhật lại customer trong danh sách _datamapping

                    // xóa driver khỏi danh sách nhưng tài xế phù hợp với yêu cầu của khách hàng
                    var driversList = _seachMapping.GetValues(response.Customer.Id);
                    driversList.RemoveAt(response.Driver.ItemIndex);
                    _seachMapping.UpdateDictionaryValue(response.Customer.Id, driversList); // cập nhật lại danh sách tài xế có thể matching với yều cầu của khách hàng

                    LocationModel model = new LocationModel();
                    model.Id = customer.Id;
                    model.Longitude = customer.Longitude;
                    model.Latitude = customer.Latitude;
                    model.Seats = customer.Seats;
                    model.Distance = customer.Distance;
                    model.Price = customer.Price;
                    model.PriceBookingId = customer.PriceBookingId;

                    string jsonLocationModel = Newtonsoft.Json.JsonConvert.SerializeObject(model); // parse object to json
                    await FindDriver(jsonLocationModel);
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

            _logger.LogInformation("---------------- End CheckAcceptedRequest Hub Function ------------------");
        }

        public async Task<bool> OpenDriver(string json) // hàm được gọi khi tài xế bật chế độ nhận yêu cầu đặt xe từ khách hàng
        {
            _logger.LogInformation("------------ Begin OpenDriver Hub Function -----------");

            LocationModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<LocationModel>(json);
            var dataHubModel = _dataMapping.GetValue(model.Id, User.Driver);
            if (dataHubModel != null)
            {
                dataHubModel.Longitude = model.Longitude;
                dataHubModel.Latitude = model.Latitude;
                dataHubModel.Status = (int)DriverStatus.On;
                _dataMapping.Add(model.Id, dataHubModel, User.Driver);
            }

            var driverResponse = await _driverService.UpdateDriverStatus(model.Id, (int)DriverStatus.On); // cập nhật status driver xuống db
            if (driverResponse.StatusCode != 201)
            {
                _logger.LogError("------------ Update Driver Status Failed to Database -----------");
                return false;
            }

            _logger.LogInformation("------------ End OpenDriver Hub Function -----------");

            return true;
        }

        public async Task<bool> CloseDriver(string driverId) // hàm được gọi khi tài xế tắt chế độ nhận yêu cầu đặt xe từ khách hàng
        {
            _logger.LogInformation("------------ Begin CloseDriver Hub Function -----------");

            _dataMapping.Remove(driverId, User.Driver);
            var driverResponse = await _driverService.UpdateDriverStatus(driverId, (int)DriverStatus.Off);
            if (driverResponse.StatusCode != 201)
            {
                _logger.LogError("------------ Update Driver Status Failed to Database -----------");
                return false;
            }

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
                _logger.LogInformation("------------ Driver Role --------------");
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
                                customerHubViewModel.Status = 1;
                                break;
                            }
                        default: break;
                    }
                }
                _dataMapping.Add(customerHubViewModel.Id, customerHubViewModel, User.Customer);
            }
            else
            {
                // Create mock data to test
                DataHubModel customerHubViewModel = new DataHubModel()
                {
                    Id = "1D17684A-00DD-4840-937B-9BC1E4DA033D",
                    FirstName = "Nam",
                    LastName = "Đào Phương",
                    Gender = "True",
                    Phone = "0369085835",
                    PhotoUrl = "",
                    Status = 1
                };
                _dataMapping.Add(customerHubViewModel.Id, customerHubViewModel, User.Customer);
                // ------------------------------------------------

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

                // Chỉ thêm những tài xế đang có xe chạy dịch vụ đặt xe
                var driver = _unitOfWork.DriverRepository.GetById(Guid.Parse(driverHubViewModel.Id)).Result;
                if (driver.VehicleId != null && _vehicleStore.VehiclesList.ContainsKey(driver.VehicleId.Value.ToString()))
                {
                    _dataMapping.Add(driverHubViewModel.Id, driverHubViewModel, User.Driver);
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
            dynamic id = Context.User.Claims.Where(x => x.Type == "CustomerId").Select(x => x.Value).FirstOrDefault();
            if (id == null)
            {
                id = Context.User.Claims.Where(x => x.Type == "DriverId").Select(x => x.Value).FirstOrDefault();
            }
            _logger.LogInformation($"OnDisconnectedAsync Context.User.Claim: {id} - {Context.ConnectionId}");

            _connections.Remove(id, Context.ConnectionId);

            _logger.LogInformation($"Number of connections of ID: {id} - ConnectionCount: {_connections.GetConnectionsCount(id)}");

            _logger.LogInformation("------------------------End On Disconnected Async--------------------------");

            return base.OnDisconnectedAsync(exception);
        }
    }
}