using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Company;
using TourismSmartTransportation.Business.SearchModel.Admin.StationManagement;
using TourismSmartTransportation.Business.SearchModel.Company.RentStationManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [Route(ApiVer1Url.Admin.RentStation)]
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
            return SendReponse(await _service.SearchRentStation(model));
        }

        // GET api/<StationMangementController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return SendReponse(await _service.GetRentStation(id));
        }

    }
}
