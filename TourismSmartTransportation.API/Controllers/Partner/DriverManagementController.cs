using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.SearchModel.Partner.DriverManagement;
using TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement;

namespace TourismSmartTransportation.API.Controllers.Partner
{
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Partner.Driver)]
    public class DriverManagementController : BaseController
    {
        private readonly IDriverManagementService _service;

        public DriverManagementController(IDriverManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Driver)]
        public async Task<IActionResult> Search([FromQuery] DriverSearchModel model)
        {
            return SendResponse(await _service.Search(model));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Driver + "/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return SendResponse(await _service.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDriverModel model)
        {
            return SendResponse(await _service.Create(model));
        }

        [HttpPut]
        [Route(ApiVer1Url.Partner.Driver + "/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDriverModel model)
        {
            return SendResponse(await _service.Update(id, model));
        }

        [HttpDelete]
        [Route(ApiVer1Url.Partner.Driver + "/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return SendResponse(await _service.Delete(id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Driver + "/vehicle-dropdown-options")]
        public async Task<IActionResult> GetVehicleListDropdownOptions([FromQuery] VehicleDropdownOptionsModel model)
        {
            return SendResponse(await _service.GetVehicleListDropdownOptions(model));
        }
    }
}