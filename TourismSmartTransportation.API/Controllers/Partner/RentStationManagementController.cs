using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Company;
using TourismSmartTransportation.Business.SearchModel.Admin.StationManagement;
using TourismSmartTransportation.Business.SearchModel.Partner.RentStationManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourismSmartTransportation.API.Controllers.Partner
{
    [Route(ApiVer1Url.Partner.RentStation)]
    [ApiController]
    [Authorize]
    public class RentStationMangementController : BaseController
    {

        private readonly IRentStationManagementService _service;

        public RentStationMangementController(IRentStationManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] RentStationSearchModel model)
        {
            return SendResponse(await _service.SearchRentStation(model));
        }

        // GET api/<StationMangementController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return SendResponse(await _service.GetRentStation(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddRentStationModel model)
        {
            return SendResponse(await _service.AddRentStation(model));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRentStation model)
        {
            return SendResponse(await _service.UpdateRentStaion(id, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return SendResponse(await _service.DeleteRentStation(id));
        }
    }
}
