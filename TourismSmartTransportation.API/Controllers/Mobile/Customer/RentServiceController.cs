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
        [HttpPost]
        [Route(ApiVer1Url.Customer.RentCustomerTrip)]
        public async Task<IActionResult> CreateCustomertrip([FromBody] CustomerTripSearchModel model)
        {
            return SendResponse(await _service.CreateCustomerTrip(model));
        }


    }
}