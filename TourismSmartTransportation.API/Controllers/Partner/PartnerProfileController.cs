using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.PartnerManagement;
using TourismSmartTransportation.Business.SearchModel.Common;
using TourismSmartTransportation.Business.ViewModel.Admin.PartnerManagement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourismSmartTransportation.API.Controllers.Partner
{
    [Route(ApiVer1Url.Partner.Profile)]
    [ApiController]
    [Authorize]
    public class PartnerProfileController : BaseController
    {
        private readonly IPartnerManagementService _service;

        public PartnerProfileController(IPartnerManagementService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPartnerById(Guid id)
        {
            return SendResponse(await _service.GetPartner(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePartnerById(Guid id, [FromForm] UpdatePartnerModel model)
        {
            return SendResponse(await _service.UpdatePartner(id, model));
        }
    }
}