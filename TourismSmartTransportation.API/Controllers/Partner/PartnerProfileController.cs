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
using TourismSmartTransportation.Business.SearchModel.Shared;
using TourismSmartTransportation.Business.ViewModel.Admin.PartnerManagement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourismSmartTransportation.API.Controllers.Partner
{
    [ApiController]
    [Authorize]
    public class PartnerProfileController : BaseController
    {
        private readonly IPartnerManagementService _service;

        public PartnerProfileController(IPartnerManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Profile + "/{id}")]
        public async Task<IActionResult> GetPartnerById(Guid id)
        {
            return SendResponse(await _service.GetPartner(id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Profile + "/history/{partnerId}")]
        public async Task<IActionResult> GetPartnerHistory(Guid partnerId)
        {
            return SendResponse(await _service.GetPartnerHistory(partnerId));
        }

        [HttpPut]
        [Route(ApiVer1Url.Partner.Profile + "/{id}")]
        public async Task<IActionResult> UpdatePartnerById(Guid id, [FromForm] UpdatePartnerProfile model)
        {
            return SendResponse(await _service.UpdatePartner(id, model));
        }

        // [HttpPut]
        // [Route(ApiVer1Url.Partner.Profile + "/change-password/{id}")]
        // public async Task<IActionResult> ChangePassword(Guid id, [FromForm] PasswordVerificationModel model)
        // {
        //     return SendResponse(await _service.ChangePassowrd(id, model));
        // }

        
    }
}