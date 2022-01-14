using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.CompanyManagement;
using TourismSmartTransportation.Business.SearchModel.Common;
using TourismSmartTransportation.Business.ViewModel.Admin.CompanyManagement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [Route(ApiVer1Url.Admin.Company)]
    [ApiController]
    public class CompanyMangementController : ControllerBase
    {

        private readonly ICompanyManagementService _service;

        public CompanyMangementController(ICompanyManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CompanySearchModel model)
        {
            return Ok(await _service.SearchCompany(model));
        }

        // GET api/<StationMangementController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _service.GetCompany(id);
            return result != null ? Ok(result) : NotFound();
        }

        // POST api/<StationMangementController>
        [HttpPost]
        [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> Post([FromForm] AddCompanyViewModel model)
        {
            return (await _service.AddCompany(model)) ? StatusCode(201) : ValidationProblem();
        }


        // PUT api/<StationMangementController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromForm] AddCompanyViewModel model)
        {
            return (await _service.UpdateCompany(id, model)) ? NoContent() : ValidationProblem();
        }

        // DELETE api/<StationMangementController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return (await _service.DeleteCompany(id)) ? Ok() : ValidationProblem();
        }
    }
}