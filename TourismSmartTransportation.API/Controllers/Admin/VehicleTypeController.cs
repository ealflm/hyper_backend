using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.VehicleType;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [ApiController]
    [Route(ApiVer1Url.Admin.VehicleType)]
    public class VehicleTypeController : BaseController
    {
        private readonly IVehicleTypeService _service;

        public VehicleTypeController(IVehicleTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetListVehicleType([FromQuery] VehicleTypeSearchModel model)
        {
            return SendResponse(await _service.GetListVehicleTypes(model));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleType(Guid id)
        {
            return SendResponse(await _service.GetVehicleType(id));
        }

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> CreateVehicleType(CreateVehicleTypeModel model)
        {
            return SendResponse(await _service.CreateVehicleType(model));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateVehicleType(Guid id, CreateVehicleTypeModel model)
        {
            return SendResponse(await _service.UpdateVehicleType(id, model));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteVehicleType(Guid id)
        {
            return SendResponse(await _service.DeleteVehicleType(id));
        }
    }
}