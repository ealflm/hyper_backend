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
    [Route(ApiVer1Url.Partner.Route)]
    [ApiController]
    public class RouteController : BaseController
    {
        private readonly IRouteManagementService _service;

        public RouteController(IRouteManagementService service)
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


        [HttpGet]
        [Route(ApiVer1Url.Partner.Route+ "/hyper-route/{partnerId}")]
        public async Task<IActionResult> GetSystemRoute(Guid partnerId)
        {
            return SendResponse(await _service.GetRouteAlready(partnerId));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Route+"/add-route-partner")]
        public async Task<IActionResult> AddRouteToPartner([FromQuery]Guid partnerId, [FromQuery]Guid routeId)
        {
            return SendResponse(await _service.AddRouteToPartner(routeId, partnerId));
        }

       [HttpPost]
        [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRouteModel model)
        {
            return SendResponse(await _service.CreateRoute(model));
        }
    }
}
