using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.Service;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [ApiController]
    [Route(ApiVer1Url.Admin.Service)]
    [Authorize]
    public class ServiceManagementController : ControllerBase
    {
        private readonly IServiceManagementService _service;
        public ServiceManagementController(IServiceManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> SearchServices([FromQuery] ServiceSearchModel model)
        {
            var result = await _service.SearchServices(model);
            if (result is null)
                return StatusCode(200, Array.Empty<object>());

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetService(Guid id)
        {
            var result = await _service.GetService(id);
            if (result.StatusCode == 404)
                return NotFound();

            if (result.StatusCode == 200)
                return Ok(result.Data);

            return Problem(result.Message, "", 500);
        }

        [HttpPost]
        [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> CreateService(CreateServiceModel model)
        {
            var result = await _service.CreateService(model);
            if (result.StatusCode == 201)
                return StatusCode(201);

            return Problem(result.Message, "", 500);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(Guid id, CreateServiceModel model)
        {
            var result = await _service.UpdateService(id, model);
            if (result.StatusCode == 204)
                return NoContent();

            if (result.StatusCode == 404)
                return NotFound();

            return Problem(result.Message, "", 500);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(Guid id)
        {
            var result = await _service.DeleteService(id);
            if (result.StatusCode == 404)
                return NotFound();

            if (result.StatusCode == 200)
                return Ok();

            return Problem(result.Message, "", 500);
        }

    }
}