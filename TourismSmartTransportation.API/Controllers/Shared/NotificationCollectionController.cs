using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Data.MongoCollections.Notification;

namespace TourismSmartTransportation.API.Controllers.Shared
{
    [ApiController]

    public class NotificationCollectionController : BaseController
    {
        private INotificationCollectionService _service;
        private IFirebaseCloudMsgService _fcmService;
        public NotificationCollectionController(INotificationCollectionService service, IFirebaseCloudMsgService fcmService)
        {
            _service = service;
            _fcmService = fcmService;
        }

        [HttpGet]
        [Route(ApiVer1Url.Customer.Notification + "/{customerId}")]
        public async Task<ActionResult<NotificationCollection>> GetNotificationsByCustomer(string customerId, string type, bool isGetAll = false)
        {
            return SendResponse(await _service.GetNotificationsByCustomer(customerId, type, isGetAll));
        }

        [HttpPut]
        [Route(ApiVer1Url.Customer.Notification + "/{customerId}")]
        public async Task<ActionResult<NotificationCollection>> DisabledNotificationStatus(string customerId, string type)
        {
            return SendResponse(await _service.DisableNotificationStatus(customerId, type));
        }
    }
}