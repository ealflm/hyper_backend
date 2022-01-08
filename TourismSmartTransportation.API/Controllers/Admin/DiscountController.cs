using System;
using System.ComponentModel.DataAnnotations;
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
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _service;

        public DiscountController(IDiscountService service)
        {
            _service = service;
        }

        // [HttpGet]
        // public async Task<IActionResult> GetListDiscounts()
        // {
        //     return Ok(await _service.GetListDiscounts());
        // }

        [HttpGet]
        public async Task<IActionResult> SearchDiscount([FromQuery] DiscountSearchModel model)
        {
            var result = await _service.SearchDiscount(model);
            if (result is null)
                return StatusCode(200, Array.Empty<Object>());

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscount(Guid id)
        {
            var entity = await _service.GetDiscount(id);
            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        [HttpPost]
        [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> CreateDiscount(CreateDiscountModel model)
        {
            var result = await _service.CreateDiscount(model);
            if (result.StatusCode == 201)
                return StatusCode(201);

            return Problem(result.Message, "", 500);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscount(Guid id, CreateDiscountModel model)
        {
            var result = await _service.UpdateDiscount(id, model);
            if (result.StatusCode == 204)
                return NoContent();

            if (result.StatusCode == 404)
                return NotFound();

            return Problem(result.Message, "", 500);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(Guid id)
        {
            var result = await _service.DeleteDiscount(id);
            if (result.StatusCode == 200)
                return Ok();

            if (result.StatusCode == 404)
                return NotFound();

            return Problem(result.Message, "", 400);
        }
    }
}