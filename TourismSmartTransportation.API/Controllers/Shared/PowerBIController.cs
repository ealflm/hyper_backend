using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TourismSmartTransportation.Business.Interfaces;
using TourismSmartTransportation.Data.MongoCollections.Vehicle;
using System;
using TourismSmartTransportation.Business.Interfaces.Shared;

namespace TourismSmartTransportation.API.Controllers
{
    [ApiController]
    public class PowerBIController : BaseController
    {

        private readonly IPowerBIService _service;

        public PowerBIController(IPowerBIService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.PowerBI)]
        public async Task<ActionResult<string>> GetToken()
        {
            return SendResponse(await _service.GetToken());
        }

       
    }
}
