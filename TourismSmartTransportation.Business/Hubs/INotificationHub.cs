using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.Hubs
{
    public interface INotificationHub
    {
        Task SendNotification(string method, object data);
    }
}