using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TourismSmartTransportation.Business.Interfaces;
using TourismSmartTransportation.Data.MongoCollections.Vehicle;
using System;
using TourismSmartTransportation.Business.Interfaces.Shared;

namespace TourismSmartTransportation.API.Controllers
{
    [ApiController]
    public class VehicleCollectionController : BaseController
    {

        private readonly IVehicleCollectionService _service;

        public VehicleCollectionController(IVehicleCollectionService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.TrackingVehicle)]
        public async Task<IEnumerable<VehicleCollection>> Get()
        {
            return await _service.GetAll();
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.TrackingVehicle + "/{id}")]
        public async Task<ActionResult<VehicleCollection>> GetById(string id)
        {
            return SendResponse(await _service.GetById(id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.TrackingVehicle + "/vehicle")]
        public async Task<ActionResult<VehicleCollection>> GetVehicleIdByAdmin([FromQuery] string vehicleId)
        {
            return SendResponse(await _service.GetByVehicleId(vehicleId));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.TrackingVehicle + "/vehicle")]
        public async Task<ActionResult<VehicleCollection>> GetVehicleIdByPartner([FromQuery] string vehicleId)
        {
            return SendResponse(await _service.GetByVehicleId(vehicleId));
        }

        [HttpPost]
        [Route(ApiVer1Url.Admin.TrackingVehicle)]
        public async Task<IActionResult> Create(AddVehicleCollectionModel Vehicle)
        {
            return SendResponse(await _service.Create(Vehicle));

        }

        // [HttpPut("{id}")]
        // public async Task<ActionResult<VehicleCollection>> Update(VehicleCollection Vehicle)
        // {
        //     var product = await _service.Update(Vehicle);
        //     return Ok();
        // }

        [HttpDelete]
        [Route(ApiVer1Url.Admin.TrackingVehicle)]
        public async Task<IActionResult> Delete([FromQuery] string id)
        {
            return SendResponse(await _service.Delete(id));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.TrackingVehicle)]
        public async Task<IEnumerable<VehicleCollection>> GetVehiclesListByPartner(Guid partnerId)
        {
            return await _service.GetVehiclesListByPartner(partnerId);
        }
    }
}
