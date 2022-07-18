using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.Package;
using TourismSmartTransportation.Business.ViewModel.Admin.PackageItem;

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [ApiController]
    [Authorize]
    public class PackageController : BaseController
    {
        private readonly IPackageService _service;

        public PackageController(IPackageService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(ApiVer1Url.Customer.Package + "/list/{customerId}")]
        public async Task<IActionResult> SearchPackage(Guid customerId)
        {
            return SendResponse(await _service.GetPackageNotUsed(customerId));
        }
    }
}