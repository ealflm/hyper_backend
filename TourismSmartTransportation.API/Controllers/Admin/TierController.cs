using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.Tier;
using TourismSmartTransportation.Business.ViewModel.Admin.Package;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [ApiController]
    [Route(ApiVer1Url.Admin.Tier)]
    public class TierController : BaseController
    {
        private readonly ITierService _service;

        public TierController(ITierService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> SearchTier([FromQuery] TierSearchModel model)
        {
            return SendResponse(await _service.SearchTier(model));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTier(Guid id)
        {
            return SendResponse(await _service.GetTier(id));
        }

        [HttpPost]
        // [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> CreateTier([FromForm] CreateTierModel model)
        {
            var formPackageList = this.Request.Form["PackageList"];
            model.PackageList = JsonExtensions.FromDelimitedJson<CreatePackageModel>(new StringReader(formPackageList)).ToList();
            return SendResponse(await _service.CreateTier(model));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTier(Guid id, [FromForm] UpdateTierModel model)
        {
            return SendResponse(await _service.UpdateTier(id, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTier(Guid id)
        {
            return SendResponse(await _service.DeleteTier(id));
        }
    }
}