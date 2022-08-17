using GeoCoordinatePortable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Hubs.Mapping;
using TourismSmartTransportation.Business.Hubs.Models;
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
        private Dictionary<string, VehicleViewModel> VehiclesList = new Dictionary<string, VehicleViewModel>();

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

        public async Task FindDriver(LocationModel model)
        {
            DataHubModel customer = _dataMapping.GetValue(model.Id, User.Customer);
            customer.Longitude = model.Longitude;
            customer.Latitude = model.Latitude;
            customer.PriceBookingId = model.PriceBookingId;
            customer.Price = model.Price;
            customer.Distance = model.Distance;
            customer.Seats = model.Seats;
            _dataMapping.Add(model.Id, customer, User.Customer);

            for (int i = customer.IntervalLoopIndex; i < DISTANCES.Count; i++) // Tìm kiếm trong những khoảng distance 3000m 5000m 7000m
            {
                foreach (var item in _dataMapping.GetDrivers(DriverStatus.On).GetItems())
                {
                    var driverId = item.Key;
                    var lng = (double)item.Value.Longitude;
                    var lat = (double)item.Value.Latitude;
                    GeoCoordinate customerCoordinates = new GeoCoordinate((double)model.Latitude, (double)model.Longitude);
                    GeoCoordinate driverCoordinates = new GeoCoordinate(lat, lng);
                    double distanceBetween = customerCoordinates.GetDistanceTo(driverCoordinates); // tính khoảng cách giữa driver và customer

                    var d = await _unitOfWork.DriverRepository.GetById(Guid.Parse(driverId)); // lấy thông tin driver
                    var vehicle = VehiclesList.GetValueOrDefault(d.VehicleId.Value.ToString()); // lấy thông tin xe mà driver đang được cấp
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
                if (driversList.Count == 0)
                {
                    continue;
                }

                // implement thuật toán tìm tài xế
                var rand = new Random();
                var index = rand.Next(0, driversList.Count - 1);
                var driver = driversList[index];
                driver.ItemIndex = index;
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
                }
            }
        }

        public List<object> GetDriversListMatching(StartLocationBookingModel model)
        {
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

            return list;
        }

        public async Task CheckAcceptedRequest(DriverResponseModel response)
        {
            // Tài xế nhận yều cầu đặt xe
            if (response.Accepted)
            {
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
                var vehicle = VehiclesList.GetValueOrDefault(driver.VehicleId.Value.ToString());

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
                    await FindDriver(model);
                }
                else
                {
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
        }

        public async Task<bool> OpenDriver(LocationModel model)
        {
            var id = Context.ConnectionId;
            var dataHubModel = _dataMapping.GetValue(model.Id, User.Driver);
            dataHubModel.Longitude = model.Longitude;
            dataHubModel.Latitude = model.Latitude;
            dataHubModel.Status = (int)DriverStatus.On;
            _dataMapping.Add(model.Id, dataHubModel, User.Driver);
            var driverResponse = await _driverService.UpdateDriverStatus(model.Id, (int)DriverStatus.On);
            if (driverResponse.StatusCode != 201)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> CloseDriver(string driverId)
        {
            // var id = Context.ConnectionId;
            _dataMapping.Remove(driverId, User.Driver);
            var driverResponse = await _driverService.UpdateDriverStatus(driverId, (int)DriverStatus.Off);
            if (driverResponse.StatusCode != 201)
            {
                return false;
            }
            return true;
        }

        public async Task LoadVehiclesList()
        {
            if (VehiclesList.Count > 0)
            {
                VehiclesList.Clear();
            }

            VehiclesList = await _unitOfWork.VehicleRepository
                            .Query()
                            .Where(x => x.ServiceTypeId.ToString() == ServiceTypeDefaultData.BOOK_SERVICE_ID)
                            .Select(x => x.AsVehicleViewModel())
                            .ToDictionaryAsync(x => x.Id.ToString());
        }

        public async Task SendToSpecificUser(string connectionId, object message)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
        }

        // ------------- Overrivde method --------------
        public override Task OnConnectedAsync()
        {
            // load vehicle có dịch vụ là đặt xe
            if (VehiclesList.Count == 0)
            {
                Task.WhenAll(LoadVehiclesList());
            }

            // Xác định id người dùng
            bool isCustomerRole = true;
            dynamic id = Context.User.Claims.Where(x => x.Type == "CustomerId").Select(x => x.Value).FirstOrDefault();
            if (id == null)
            {
                id = Context.User.Claims.Where(x => x.Type == "DriverId").Select(x => x.Value).FirstOrDefault();
                isCustomerRole = false;
            }
            _logger.LogInformation($"OnConnectedAsync Context.User.Claim: {id} - {Context.ConnectionId}");

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
                                driverHubViewModel.Status = (int)DriverStatus.Off;
                                break;
                            }
                        default: break;
                    }
                }

                // Chỉ thêm những tài xế đang có xe chạy dịch vụ đặt xe
                var driver = _unitOfWork.DriverRepository.GetById(Guid.Parse(driverHubViewModel.Id)).Result;
                if (driver.VehicleId != null && VehiclesList.ContainsKey(driver.VehicleId.Value.ToString()))
                {
                    _dataMapping.Add(driverHubViewModel.Id, driverHubViewModel, User.Driver);
                }
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            dynamic id = Context.User.Claims.Where(x => x.Type == "CustomerId").Select(x => x.Value).FirstOrDefault();
            if (id == null)
            {
                id = Context.User.Claims.Where(x => x.Type == "DriverId").Select(x => x.Value).FirstOrDefault();
            }
            _logger.LogInformation($"OnDisconnectedAsync Context.User.Claim: {id} - {Context.ConnectionId}");

            _connections.Remove(id, Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
    }
}