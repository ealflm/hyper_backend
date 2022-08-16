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
    
    public class BookingServiceController : BaseController
    {
        private readonly IBookingService _service;

        public BookingServiceController(IBookingService service)
        {
            _service = service;
        }


        [HttpGet]
        [Route(ApiVer1Url.Customer.Booking)]
        public async Task<IActionResult> GetPrice([FromQuery] decimal distance, [FromQuery] int seat)
        {
            return SendResponse(await _service.GetPrice(distance, seat));
        }
        

    }
}