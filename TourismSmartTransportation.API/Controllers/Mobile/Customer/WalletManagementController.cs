using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Customer.Wallet)]
    public class WalletManagementController : BaseController
    {
        private readonly IWalletManagementService _service;

        public WalletManagementController(IWalletManagementService service)
        {
            _service = service;
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetWalletByCustomer(Guid customerId)
        {
            return SendResponse(await _service.GetWalletByCustomer(customerId));
        }
    }
}