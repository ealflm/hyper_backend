using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendToAll(object message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        public async Task SendToSpecificUser(string connectionId, object message)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
        }

        public async Task Send(string name, string message)
        {
            await Clients.All.SendAsync("OnMessage", name, message);
        }
    }
}