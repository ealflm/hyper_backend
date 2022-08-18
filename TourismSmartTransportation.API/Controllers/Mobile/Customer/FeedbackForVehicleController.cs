using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.MoMo;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [ApiController]
    [Authorize]

    [Route(ApiVer1Url.Customer.FeebackForVehicle)]
    public class FeedbackForVehicleController : BaseController
    {
        private readonly IFeedbackForVehicleService _service;

        public FeedbackForVehicleController(IFeedbackForVehicleService service)
        {
            _service = service;
        }


        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetPrice(Guid customerId)
        {
            return SendResponse(await _service.GetFeedBackByCustomerTripId(customerId));
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomertrip([FromBody] FeedbackForVehicleSearchModel model)
        {
            return SendResponse(await _service.CreateFeedback(model));
        }
       

    }
}