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

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [Route(ApiVer1Url.Customer.Station)]
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
        public async Task<IActionResult> Get([FromQuery] StationSearchModel model)
        {
            return SendResponse(await _service.SearchStation(model));
        }

    }
}
