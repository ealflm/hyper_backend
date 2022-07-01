using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.StationManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourismSmartTransportation.API.Controllers.Partner
{
    [Route(ApiVer1Url.Partner.Station)]
    [ApiController]
    [Authorize]
    public class StationMangementController : BaseController
    {

        private readonly IStationManagementService _service;

        public StationMangementController(IStationManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetStationList([FromQuery] StationSearchModel model)
        {
            return SendResponse(await _service.SearchStation(model));
        }

        // GET api/<StationMangementController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStationById(Guid id)
        {
            return SendResponse(await _service.GetStation(id));
        }
    }
}
