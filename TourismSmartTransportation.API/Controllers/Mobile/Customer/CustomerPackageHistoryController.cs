using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [ApiController]
    [Authorize]
    public class CustomerPackageHistoryController : BaseController
    {
        private readonly ICustomerPackagesHistoryService _service;
        public CustomerPackageHistoryController(ICustomerPackagesHistoryService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.CustomerPackageHistory)]
        public async Task<IActionResult> GetByAdmin([FromQuery] CustomerPackagesHistorySearchModel model)
        {
            return SendResponse(await _service.GetCustomerPackageHistory(model));
        }

        [HttpGet]
        [Route(ApiVer1Url.Customer.CustomerPackageHistory)]
        public async Task<IActionResult> GetByCustomer([FromQuery] CustomerPackagesHistorySearchModel model)
        {
            return SendResponse(await _service.GetCustomerPackageHistory(model));
        }

        [HttpGet]
        [Route(ApiVer1Url.Customer.PackageDetails)]
        public async Task<IActionResult> GetPackageDetails([FromQuery] PackageDetailsSearchModel model)
        {
            return SendResponse(await _service.GetPackageDetails(model));
        }
    }
}