using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.Hubs
{
    public class NotificationHub : Hub<INotificationHub>
    {
        public async Task SendNotificationToAllUsers(object message)
        {
            await Clients.All.SendNotification("ReceiveNotification", message);
        }

        public async Task SendNotificationToSpecificUser(string connectionId, object message)
        {
            await Clients.Client(connectionId).SendNotification("ReceiveNotification", message);
        }
    }
}