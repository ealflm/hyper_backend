using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.Discount;
using TourismSmartTransportation.Business.SearchModel.Admin.PriceBusServiceConfig;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Admin.PriceBusConfig)]
    public class PriceBusConfigController : BaseController
    {
        private readonly IPriceBusServiceConfig _service;

        public PriceBusConfigController(IPriceBusServiceConfig service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] PriceBusServiceSearchModel model)
        {
            return SendResponse(await _service.Search(model));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return SendResponse(await _service.GetById(id));
        }

        // [HttpPost]
        // // [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        // public async Task<IActionResult> Create([FromBody] CreatePriceBusServiceModel model)
        // {
        //     return SendResponse(await _service.CreatePrice(model));
        // }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePriceBusServiceModel model)
        {
            return SendResponse(await _service.UpdatePrice(id, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return SendResponse(await _service.DeletePrice(id));
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePriceBaseOnBasePrice([FromBody] AddBasePriceOfBusService model)
        {
            return SendResponse(await _service.GeneratePriceOfBusService(model));
        }
    }
}