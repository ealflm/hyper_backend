using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.Discount;
using TourismSmartTransportation.Business.SearchModel.Admin.PriceRentingServiceConfig;
using TourismSmartTransportation.Business.SearchModel.Admin.PriceBusServiceConfig;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Admin.PriceRentingConfig)]
    public class PriceRentingConfigController : BaseController
    {
        private readonly IPriceRentingServiceConfig _service;

        public PriceRentingConfigController(IPriceRentingServiceConfig service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] PriceRentingServiceSearchModel model)
        {
            return SendResponse(await _service.Search(model));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return SendResponse(await _service.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePriceRentingServiceModel model)
        {
            return SendResponse(await _service.CreatePrice(model));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePriceRentingServiceModel model)
        {
            return SendResponse(await _service.UpdatePrice(id, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return SendResponse(await _service.DeletePrice(id));
        }
    }
}