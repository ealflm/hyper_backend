using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.SearchModel.Admin.Package;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Admin.PackageItem;

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [ApiController]
    [Authorize]
    public class PackageController : BaseController
    {
        private readonly IPackageService _service;
        private readonly IBuyPackageService _buyPackageService;

        public PackageController(IPackageService service, IBuyPackageService buyPackageService)
        {
            _service = service;
            _buyPackageService = buyPackageService;
        }

        [HttpGet]
        [Route(ApiVer1Url.Customer.Package + "/list")]
        public async Task<IActionResult> SearchPackage([FromQuery] PackageCustomerModel model)
        {
            // return SendResponse(await _service.GetPackageNotUsed(model));
            return SendResponse(await _service.GetAvailablePackage(model));
        }

        [HttpGet]
        [Route(ApiVer1Url.Customer.Package + "/current-package")]
        public async Task<IActionResult> GetCurrentPackageIsUsed([FromQuery] Guid customerId)
        {
            return SendResponse(await _service.GetCurrentPackageIsUsed(customerId));
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.Package + "/package-purchase")]
        public async Task<IActionResult> PurchasePackage(BuyPackageModel model)
        {
            return SendResponse(await _buyPackageService.BuyPackage(model));
        }
    }
}