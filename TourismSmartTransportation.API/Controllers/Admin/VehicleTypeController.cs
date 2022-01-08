using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.Vehicle;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [ApiController]
    [Route(ApiVer1Url.Admin.VehicleType)]
    public class VehicleTypeController : ControllerBase
    {
        private readonly IVehicleTypeService _service;

        public VehicleTypeController(IVehicleTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetListVehicleType()
        {
            return Ok(await _service.GetListVehicleTypes());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleType(Guid id)
        {
            var result = await _service.GetVehicleType(id);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> CreateVehicleType(CreateVehicleModel model)
        {
            var result = await _service.CreateVehicleType(model);
            if (result.StatusCode == 201)
                return StatusCode(201);

            return Problem(result.Message, "", 500);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateVehicleType(Guid id, VehicleTypeSearchModel model)
        {
            var result = await _service.UpdateVehicleType(id, model);
            if (result.StatusCode == 204)
                return NoContent();

            if (result.StatusCode == 404)
                return NotFound();

            return Problem(result.Message, "", 500);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteVehicleType(Guid id)
        {
            var result = await _service.DeleteVehicleType(id);
            if (result.StatusCode == 200)
                return Ok();

            if (result.StatusCode == 404)
                return NotFound();

            return Problem(result.Message, "", 500);
        }
    }
}