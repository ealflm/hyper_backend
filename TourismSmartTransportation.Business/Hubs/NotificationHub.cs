using GeoCoordinatePortable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Partner;

namespace TourismSmartTransportation.Business.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;
        private readonly IDriverManagementService _driverService;
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>(); // Tạo instance connection mapping
        private readonly static DataMapping _dataMapping = new DataMapping(); // Tạo instance lưu danh sách thông tin customer và danh sách thông tin driver

        public NotificationHub(ILogger<NotificationHub> logger, IDriverManagementService driverService)
        {
            _logger = logger;
            _driverService = driverService;
        }

        public async Task FindDriver(DataHubModel data)
        {
            var connectionId = Context.ConnectionId;
            _dataMapping.Add(data.Id, data, isCustomer: true);
            // implement code for find driver
            var list = new List<Tuple<string, double, DataHubModel>>();
            foreach (var item in _dataMapping.GetDrivers().GetItems())
            {
                var driverId = item.Key;
                var lng = (double)item.Value.Longitude;
                var lat = (double)item.Value.Latitude;
                GeoCoordinate customerCoordinates = new GeoCoordinate((double)data.Latitude, (double)data.Longitude);
                GeoCoordinate driverCoordinates = new GeoCoordinate(lat, lng);
                double distanceBetween = customerCoordinates.GetDistanceTo(driverCoordinates);
                if (distanceBetween <= 3000)
                {
                    list.Add(Tuple.Create(driverId, distanceBetween, item.Value));
                }
            }

            if (list.Count == 0)
            {
                await Clients.Client(connectionId).SendAsync("FindResult", new { statusCode = 200, Items = list });
                return;
            }

            list = list.OrderByDescending(x => x.Item2).ToList();
            // implement thuật toán tìm tài xế
        }

        public async Task OpenDriver(DataHubModel data)
        {
            var id = Context.ConnectionId;
            _dataMapping.Add(data.Id, data, isCustomer: false);
            var driverResponse = await _driverService.UpdateDriverStatus(data.Id, (int)DriverStatus.On);
            if (driverResponse.StatusCode != 201)
            {
                await Clients.Client(id).SendAsync("ErrorMessage", driverResponse);
                return;
            }
            await Clients.Client(id).SendAsync("Message", "Đã bật chế độ nhận yêu cầu đặt xe");
        }

        public async Task CloseDriver(string driverId)
        {
            var id = Context.ConnectionId;
            _dataMapping.Remove(driverId, isCustomer: false);
            var driverResponse = await _driverService.UpdateDriverStatus(driverId, (int)DriverStatus.Off);
            if (driverResponse.StatusCode != 201)
            {
                await Clients.Client(id).SendAsync("ErrorMessage", driverResponse);
                return;
            }
            await Clients.Client(id).SendAsync("Message", "Đã tắt chế độ nhận yêu cầu đặt xe");
        }

        public async Task SendToSpecificUser(string connectionId, object message)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
        }

        // ------------- Overrivde method --------------
        public override Task OnConnectedAsync()
        {
            // Xác định id người dùng
            dynamic id = Context.User.Claims.Where(x => x.Type == "CustomerId").Select(x => x.Value).FirstOrDefault();
            if (id == null)
            {
                id = Context.User.Claims.Where(x => x.Type == "DriverId").Select(x => x.Value).FirstOrDefault();
            }
            _logger.LogInformation($"OnConnectedAsync Context.User.Claim: {id} - {Context.ConnectionId}");

            _connections.Add(id, Context.ConnectionId); // lưu id người dùng và id máy người dùng (connectionId)

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