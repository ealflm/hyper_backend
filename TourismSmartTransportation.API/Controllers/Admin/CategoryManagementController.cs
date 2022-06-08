using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.CategoryManagement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [Authorize]
    [Route(ApiVer1Url.Admin.Category)]
    [ApiController]
    public class CategoryManagementController : BaseController
    {

        private readonly ICategoryManagementService _service;

        public CategoryManagementController(ICategoryManagementService service)
        {
            _service = service;
        }


        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return SendResponse(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return SendResponse(await _service.Get(id));
        }

        // POST api/<CategoryManagementController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] CategorySearchModel model)
        {
            return SendResponse(await _service.Add(model));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromForm] CategorySearchModel model)
        {
            return SendResponse(await _service.Update(id, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return SendResponse(await _service.Delete(id));
        }
    }
}
