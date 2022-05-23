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

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [Route(ApiVer1Url.Admin.ServiceType)]
    [Authorize]
    [ApiController]
    public class ServiceTypeManagementController : BaseController
    {
        private readonly IServiceTypeManagementService _service;

        public ServiceTypeManagementController(IServiceTypeManagementService service)
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

        [HttpPost]
        [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> Create(ServiceTypeSearchModel model)
        {
            return SendResponse(await _service.Create(model));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiscount(Guid id, ServiceTypeSearchModel model)
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
