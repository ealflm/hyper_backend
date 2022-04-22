using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.Discount;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Admin.Discount)]
    public class DiscountController : BaseController
    {
        private readonly IDiscountService _service;

        public DiscountController(IDiscountService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> SearchDiscount([FromQuery] DiscountSearchModel model)
        {
            return SendResponse(await _service.SearchDiscount(model));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscount(Guid id)
        {
            return SendResponse(await _service.GetDiscount(id));
        }

        [HttpPost]
        [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> CreateDiscount(CreateDiscountModel model)
        {
            return SendResponse(await _service.CreateDiscount(model));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscount(Guid id, CreateDiscountModel model)
        {
            return SendResponse(await _service.UpdateDiscount(id, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(Guid id)
        {
            return SendResponse(await _service.DeleteDiscount(id));
        }
    }
}