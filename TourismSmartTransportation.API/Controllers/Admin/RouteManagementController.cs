using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.SearchModel.Partner.Route;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [Route(ApiVer1Url.Admin.Route)]
    [ApiController]
    public class RouteManagementController : BaseController
    {
        private readonly IRouteManagementService _service;

        public RouteManagementController(IRouteManagementService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] RouteSearchModel model)
        {
            return SendResponse(await _service.GetAll(model));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoute(Guid id)
        {
            return SendResponse(await _service.GetRouteById(id));
        }
    }
}
