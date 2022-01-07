using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.StationManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [Route(ApiVer1Url.Admin.Station)]
    [ApiController]
    [Authorize]
    public class StationMangementController : ControllerBase
    {

        private readonly IStationManagementService _service;

        public StationMangementController(IStationManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] StationSearchModel model)
        {
            return Ok(await _service.SearchStation(model));
        }

        // GET api/<StationMangementController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _service.GetStation(id);
            return result !=null ? Ok(result): NotFound();
        }

        // POST api/<StationMangementController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddStationViewModel model) 
        {
            return (await _service.AddStation(model)) ? StatusCode(201) : ValidationProblem();
        }
        

        // PUT api/<StationMangementController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] AddStationViewModel model)
        {
            return (await _service.UpdateStation(id, model)) ? NoContent() : ValidationProblem();
        }

        // DELETE api/<StationMangementController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return (await _service.DeleteStation(id)) ? Ok() : ValidationProblem();
        }
    }
}
