using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.CustomerManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.CustomerManagement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [Route(ApiVer1Url.Admin.Customer)]
    [ApiController]
    public class CustomerManagementController : ControllerBase
    {
        private readonly ICustomerManagementService _service;

        public CustomerManagementController(ICustomerManagementService service)
        {
            _service = service;
        }


        // GET: api/<CustomerManagementController>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CustomerSearchModel model)
        {
            return Ok(await _service.SearchCustomer(model));
        }

        // GET api/<CustomerManagementController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _service.GetCustomer(id);
            return result != null ? Ok(result) : NotFound();
        }

        // POST api/<CustomerManagementController>
        [HttpPost]
        [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> Post([FromForm] AddCustomerViewModel model)
        {
            return (await _service.AddCustomer(model)) ? StatusCode(201) : ValidationProblem();
        }

        // PUT api/<CustomerManagementController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromForm] AddCustomerViewModel model)
        {
            return (await _service.UpdateCustomer(id, model)) ? NoContent() : ValidationProblem();
        }

        // DELETE api/<CustomerManagementController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return (await _service.DeleteCustomer(id)) ? Ok() : ValidationProblem();
        }
    }
}
