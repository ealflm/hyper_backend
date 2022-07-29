using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [ApiController]
    [Authorize]
    public class BusTripController : BaseController
    {
        private readonly IBusTripService _service;

        public BusTripController(IBusTripService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.BusTrip)]
        public async Task<IActionResult> FindBusTrip([FromBody] BusTripSearchModel model)
        {
            return SendResponse(await _service.FindBusTrip(model));
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.PayBusTripMobile)]
        public async Task<IActionResult> PayWithMobileApp([FromForm] BusPaySearchModel model)
        {
            return SendResponse(await _service.PayWithMobileApp(model));
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.PayBusTripIOT)]
        public async Task<IActionResult> PayWithIOT([FromForm] BusPaySearchModel model)
        {
            return SendResponse(await _service.PayWithIOT(model));
        }
    }
}