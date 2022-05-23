using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.SearchModel.Shared;

namespace TourismSmartTransportation.API.Controllers.Shared
{
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Admin.CustomerTierHistory)]
    public class CustomerTierHistoryController : BaseController
    {
        private readonly ICustomerTierHistoryService _service;
        public CustomerTierHistoryController(ICustomerTierHistoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CustomerTierHistorySearchModel model)
        {
            return SendResponse(await _service.Get(model));
        }
    }
}