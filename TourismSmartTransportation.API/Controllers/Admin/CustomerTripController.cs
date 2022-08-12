using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.MoMo;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Admin.CustomerTrip)]
    public class CustomerTripController : BaseController
    {
        private readonly ICustomerTripService _service;

        public CustomerTripController(ICustomerTripService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetCustomerTrip()
        {
            return SendResponse(await _service.GetCustomerTrips());
        }


    }
}