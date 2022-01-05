using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Vehicle;

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
            var list = await _service.GetListVehicleTypes();
            return Ok(list);
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
        public async Task<IActionResult> CreateVehicleType(VehicleTypeSearchModel model)
        {
            var result = await _service.CreateVehicleType(model);
            if (result)
                return StatusCode(201);

            return BadRequest();
        }

        [HttpPut("id")]
        [Authorize]
        public async Task<IActionResult> UpdateVehicleType(Guid id, VehicleTypeSearchModel model)
        {
            var result = await _service.UpdateVehicleType(id, model);
            if (result)
                return NoContent();

            return BadRequest();
        }

        [HttpDelete("id")]
        [Authorize]
        public async Task<IActionResult> DeleteVehicleType(Guid id)
        {
            var result = await _service.DeleteVehicleType(id);
            if (result)
                return StatusCode(200);

            return BadRequest();
        }
    }
}