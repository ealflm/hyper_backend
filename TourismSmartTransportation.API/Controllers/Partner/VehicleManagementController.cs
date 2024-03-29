using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement;

namespace TourismSmartTransportation.API.Controllers.Partner
{
    [ApiController]
    [Authorize]
    public class VehicleManagementController : BaseController
    {
        private readonly IVehicleManagementService _service;

        public VehicleManagementController(IVehicleManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Vehicle)]
        public async Task<IActionResult> Search([FromQuery] VehicleSearchModel model)
        {
            return SendResponse(await _service.Search(model));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Vehicle + "/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return SendResponse(await _service.GetById(id));
        }

        [HttpPost]
        [Route(ApiVer1Url.Partner.Vehicle)]
        public async Task<IActionResult> Create([FromBody] CreateVehicleModel model)
        {
            return SendResponse(await _service.Create(model));
        }

        [HttpPut]
        [Route(ApiVer1Url.Partner.Vehicle + "/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehicleModel model)
        {
            return SendResponse(await _service.Update(id, model));
        }

        [HttpDelete]
        [Route(ApiVer1Url.Partner.Vehicle + "/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return SendResponse(await _service.Delete(id));
        }
    }
}