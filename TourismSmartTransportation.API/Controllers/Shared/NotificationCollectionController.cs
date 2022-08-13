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
        public NotificationCollectionController(INotificationCollectionService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(ApiVer1Url.Customer.Notification + "/{customerId}")]
        public async Task<ActionResult<NotificationCollection>> GetNotificationsByCustomer(string customerId)
        {
            return SendResponse(await _service.GetNotificationsByCustomer(customerId));
        }
    }
}