using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.SearchModel.Shared;

namespace TourismSmartTransportation.API.Controllers.Shared
{
    [ApiController]
    public class FirebaseServiceController : BaseController
    {
        private readonly IFirebaseCloudMsgService _firebaseService;

        public FirebaseServiceController(IFirebaseCloudMsgService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.Firebase + "/fcm/registrationToken")]
        public async Task<IActionResult> SaveRegistrationToken(FcmRegistrationTokenModel model)
        {
            return SendResponse(await _firebaseService.SaveRegistrationToken(model, isCustomer: true));
        }

        [HttpPost]
        [Route(ApiVer1Url.Driver.Firebase + "/fcm/registrationToken")]
        public async Task<IActionResult> SaveRegistrationTokenDriver(FcmRegistrationTokenModel model)
        {
            return SendResponse(await _firebaseService.SaveRegistrationToken(model, isDriver: true));
        }
    }
}