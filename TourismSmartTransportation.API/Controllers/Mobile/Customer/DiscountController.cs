using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Admin;

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [ApiController]
    [Authorize]
    public class DiscountController : BaseController
    {
        private readonly IDiscountService _service;
        public DiscountController(IDiscountService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.Discount + "/verification")]
        public async Task<IActionResult> CheckAvaiableDiscount([FromBody] string discountCode)
        {
            return SendResponse(await _service.CheckAvaliableDiscount(discountCode));
        }
    }
}