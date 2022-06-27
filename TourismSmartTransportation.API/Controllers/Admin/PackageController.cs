using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.Package;
using TourismSmartTransportation.Business.ViewModel.Admin.Package;
using TourismSmartTransportation.Business.ViewModel.Admin.PackageItem;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [ApiController]
    [Route(ApiVer1Url.Admin.Package)]
    public class PackageController : BaseController
    {
        private readonly IPackageService _service;

        public PackageController(IPackageService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> SearchPackage([FromQuery] PackageSearchModel model)
        {
            return SendResponse(await _service.SearchPackage(model));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackage(Guid id)
        {
            return SendResponse(await _service.GetPackage(id));
        }

        [HttpPost]
        // [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> CreatePackage([FromForm] CreatePackageModel model)
        {
            var formPackageList = this.Request.Form["PackageList"];
            model.PackageItems = JsonExtensions.FromDelimitedJson<CreatePackageItemModel>(new StringReader(formPackageList)).ToList();
            return SendResponse(await _service.CreatePackage(model));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePackage(Guid id, [FromForm] UpdatePackageModel model)
        {
            var formPackageList = this.Request.Form["PackageList"];
            model.PackageItems = JsonExtensions.FromDelimitedJson<UpdatePackageItemModel>(new StringReader(formPackageList)).ToList();
            return SendResponse(await _service.UpdatePackage(id, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackage(Guid id)
        {
            return SendResponse(await _service.DeletePackage(id));
        }
    }
}