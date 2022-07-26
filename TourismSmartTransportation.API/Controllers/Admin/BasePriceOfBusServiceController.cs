using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.BasePriceOfBusService;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Admin.BasePriceOfBusService)]
    public class BasePriceOfBusServiceController : BaseController
    {
        private readonly IBasePriceOfBusService _service;
        public BasePriceOfBusServiceController(IBasePriceOfBusService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetBasePricesList([FromQuery] BasePriceOfBusServiceSearchModel model)
        {
            return SendResponse(await _service.GetBasePricesList(model));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBasePriceById(Guid id)
        {
            return SendResponse(await _service.GetBasePricesById(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBasePrice(AddBasePriceOfBusService model)
        {
            return SendResponse(await _service.GeneratePriceOfBusService(model));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBasePrice(Guid id, UpdateBasePriceOfBusService model)
        {
            return SendResponse(await _service.UpdateBasePrice(id, model));
        }
    }
}