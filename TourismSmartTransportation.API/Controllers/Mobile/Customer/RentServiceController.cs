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
    
    public class RentServiceController : BaseController
    {
        private readonly IRentService _service;

        public RentServiceController(IRentService service)
        {
            _service = service;
        }


        [HttpPost]
        [Route(ApiVer1Url.Customer.RentService)]
        public async Task<IActionResult> GetPrice([FromForm]string id)
        {
            return SendResponse(await _service.GetPrice(id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Customer.RentService + "/extend/{customerTripId}")]
        public async Task<IActionResult> GetPriceExtend(Guid customerTripId)
        {
            return SendResponse(await _service.GetPriceExtend(customerTripId));
        }

        [HttpGet]
        [Route(ApiVer1Url.Customer.RentService + "/extend")]
        public async Task<IActionResult> CheckMergeOrder([FromQuery] int time, [FromQuery] Guid customerTripId)
        {
            return SendResponse(await _service.CheckMergeOrder(time, customerTripId));
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.RentService + "/extend")]
        public async Task<IActionResult> CreateCustomertrip([FromBody] ExtendOrderSearchModel model)
        {
            return SendResponse(await _service.ExtendRentingOrder(model));
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.RentCustomerTrip)]
        public async Task<IActionResult> CreateCustomertrip([FromBody] CustomerTripSearchModel model)
        {
            return SendResponse(await _service.CreateCustomerTrip(model));
        }
        [HttpPost]
        [Route(ApiVer1Url.Customer.ReturnVehicle)]
        public async Task<IActionResult> ReturnVehicle([FromBody] ReturnVehicleSearchModel model)
        {
            return SendResponse(await _service.ReturnVehicle(model));
        }

    }
}