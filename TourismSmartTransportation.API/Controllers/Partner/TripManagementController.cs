using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.SearchModel.Partner.DriverManagement;
using TourismSmartTransportation.Business.SearchModel.Partner.Route;
using TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourismSmartTransportation.API.Controllers.Partner
{
    [ApiController]
    [Authorize]
    public class TripManagementController : BaseController
    {
        private readonly ITripManagementService _service;

        public TripManagementController(ITripManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Trip)]
        public async Task<IActionResult> GetTripsList([FromQuery] TripSearchModel model)
        {
            return SendResponse(await _service.GetTripsList(model));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Trip + "/{id}")]
        public async Task<IActionResult> GetTripById(Guid id)
        {
            return SendResponse(await _service.GetTripById(id));
        }


        [HttpPost]
        [Route(ApiVer1Url.Partner.Trip + "/copy-trip")]
        public async Task<IActionResult> CopyTrip([FromBody] CopyTripModel model)
        {
            return SendResponse(await _service.CopyTrip(model));
        }

        [HttpPost]
        [Route(ApiVer1Url.Partner.Trip)]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripModel model)
        {
            return SendResponse(await _service.CreateTrip(model));
        }

        [HttpPut]
        [Route(ApiVer1Url.Partner.Trip + "/{id}")]
        public async Task<IActionResult> UpdateTrip(Guid id, UpdateTripModel model)
        {
            return SendResponse(await _service.UpdateTrip(id, model));
        }

        [HttpDelete]
        [Route(ApiVer1Url.Partner.Trip + "/{id}")]
        public async Task<IActionResult> DeleteTrip(Guid id)
        {
            return SendResponse(await _service.DeleteTrip(id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Trip + "/vehicle-dropdown-options")]
        public async Task<IActionResult> GetVehicleListDropdownOptions([FromQuery] VehicleDropdownOptionsTripModel model)
        {
            return SendResponse(await _service.GetVehicleListDropdownOptions(model));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Trip + "/driver-dropdown-options")]
        public async Task<IActionResult> GetDriverListDropdownOptions([FromQuery] DriverDropdownOptionsTripModel model)
        {
            return SendResponse(await _service.GetDriverListDropdownOptions(model));
        }
    }
}
