using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.ServiceType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourismSmartTransportation.API.Controllers.Partner
{
    [Route(ApiVer1Url.Partner.ServiceType)]
    [Authorize]
    [ApiController]
    public class ServiceTypeManagementController : BaseController
    {
        private readonly IServiceTypeManagementService _service;

        public ServiceTypeManagementController(IServiceTypeManagementService service)
        {
            _service = service;
        }


        [HttpGet("{partnerId}")]
        public async Task<IActionResult> GetListByPartnerId(Guid partnerId)
        {
            return SendResponse(await _service.GetListByPartnerId(partnerId));
        }

    }
}
