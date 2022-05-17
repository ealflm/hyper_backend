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

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [Route(ApiVer1Url.Admin.Partner)]
    [ApiController]
    public class PartnerMangementController : BaseController
    {

        private readonly IPartnerManagementService _service;

        public PartnerMangementController(IPartnerManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PartnerSearchModel model)
        {
            return SendResponse(await _service.SearchPartner(model));
        }

        // GET api/<StationMangementController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return SendResponse(await _service.GetPartner(id));
        }

        // POST api/<StationMangementController>
        [HttpPost]
        [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> Post([FromForm] AddPartnerModel model)
        {
            return SendResponse(await _service.AddPartner(model));
        }


        // PUT api/<StationMangementController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromForm] AddPartnerModel model)
        {
            return SendResponse(await _service.UpdatePartner(id, model));
        }

        // DELETE api/<StationMangementController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return SendResponse(await _service.DeletePartner(id));
        }
    }
}