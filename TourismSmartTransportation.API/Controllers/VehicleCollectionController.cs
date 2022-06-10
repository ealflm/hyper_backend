using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TourismSmartTransportation.Business.Interfaces;
using TourismSmartTransportation.Data.MongoCollections;

namespace TourismSmartTransportation.API.Controllers
{
    [ApiController]
    [Route(ApiVer1Url.Admin.TrackingVehicle)]
    public class VehicleCollectionController : BaseController
    {

        private readonly IVehicleCollectionService _service;

        public VehicleCollectionController(IVehicleCollectionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<VehicleCollection>> Get()
        {
            return await _service.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VehicleCollection>> GetById(string id)
        {
            return SendResponse(await _service.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create(VehicleCollection Vehicle)
        {
            return SendResponse(await _service.Create(Vehicle));

        }

        [HttpPut]
        public async Task<ActionResult<VehicleCollection>> Update(VehicleCollection Vehicle)
        {
            var product = await _service.Update(Vehicle);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return SendResponse(await _service.Delete(id));
        }
    }
}
