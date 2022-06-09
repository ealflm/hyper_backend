using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TourismSmartTransportation.Business.Interfaces;
using TourismSmartTransportation.Data.MongoCollections;

namespace TourismSmartTransportation.API.Controllers
{
    [ApiController]
    [Route(ApiVer1Url.Admin.BaseApiUrl + "/driver-tracking")]
    public class DriverCollectionController : BaseController
    {

        private readonly IDriverCollectionService _service;

        public DriverCollectionController(IDriverCollectionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<DriverCollection>> Get()
        {
            return await _service.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DriverCollection>> GetById(string id)
        {
            return SendResponse(await _service.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create(DriverCollection driver)
        {
            return SendResponse(await _service.Create(driver));

        }

        [HttpPut]
        public async Task<ActionResult<DriverCollection>> Update(DriverCollection driver)
        {
            var product = await _service.Update(driver);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return SendResponse(await _service.Delete(id));
        }
    }
}
