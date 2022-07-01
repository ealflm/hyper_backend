using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.SearchModel.Partner.Route;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourismSmartTransportation.API.Controllers.Partner
{
    [Route(ApiVer1Url.Partner.Trip)]
    [ApiController]
    public class TripManagementController : BaseController
    {
        private readonly ITripManagementService _service;

        public TripManagementController(ITripManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetTripsList([FromQuery] TripSearchModel model)
        {
            return SendResponse(await _service.GetTripsList(model));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripById(Guid id)
        {
            return SendResponse(await _service.GetTripById(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripModel model)
        {
            return SendResponse(await _service.CreateTrip(model));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrip(Guid id, UpdateTripModel model)
        {
            return SendResponse(await _service.UpdateTrip(id, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(Guid id)
        {
            return SendResponse(await _service.DeleteTrip(id));
        }
    }
}
