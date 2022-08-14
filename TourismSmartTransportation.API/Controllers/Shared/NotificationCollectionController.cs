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
        public async Task<ActionResult<NotificationCollection>> GetNotificationsByCustomer(string customerId)
        {
            return SendResponse(await _service.GetNotificationsByCustomer(customerId));
        }

        [HttpGet]
        [Route(ApiVer1Url.Customer.Notification + "/test")]
        public async Task<ActionResult<NotificationCollection>> Test()
        {
            string clientToken = "fO45R12SSVi8o6R4_s2a4A:APA91bFORcpmZ2ZmbWrDIe0dBSo6eqKG7xXFIzT9GDs0of64vnzkz_USKjw8cEArUpSxkOvud_iZ15zAJhOqC_8fya7lKh_o2EnfbKEf13qob5TheGK8UY4qlSwlVsm4ET11gs_78jHN";
            string title = "Test";
            string message = $"Thời gian thuê xe của quý khách sẽ hết hạn lúc";
            await _fcmService.SendNotificationForRentingService(clientToken, title, message);
            return Ok();
        }
    }
}