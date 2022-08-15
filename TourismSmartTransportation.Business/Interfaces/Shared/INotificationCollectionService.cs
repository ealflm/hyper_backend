using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Shared.NotificationCollection;
using TourismSmartTransportation.Data.MongoCollections.Notification;

namespace TourismSmartTransportation.Business.Interfaces.Shared
{
    public interface INotificationCollectionService
    {
        Task<List<NotificationCollection>> GetNotificationsByCustomer(string customerId, string type);
        Task<Response> SaveNotification(SaveNotificationModel noti);
        Task<Response> DisableNotificationStatus(string customerId, string type);
    }
}