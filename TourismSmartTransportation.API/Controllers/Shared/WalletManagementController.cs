using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.Interfaces.Shared;

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [ApiController]
    [Authorize]
    public class WalletManagementController : BaseController
    {
        private readonly IWalletManagementService _service;
        private readonly ICustomerManagementService _customerService;
        private readonly IPartnerManagementService _partnerService;

        public WalletManagementController(IWalletManagementService service, ICustomerManagementService customerService, IPartnerManagementService partnerService)
        {
            _service = service;
            _customerService = customerService;
            _partnerService = partnerService;
        }

        [HttpGet]
        [Route(ApiVer1Url.Customer.Wallet + "/{id}")]
        public async Task<IActionResult> GetWalletByCustomer(Guid id)
        {
            var customer = await _customerService.GetCustomer(id);
            if (customer != null)
            {
                return SendResponse(await _service.GetWallet(id));
            }
            return SendResponse(customer);
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Wallet + "/{id}")]
        public async Task<IActionResult> GetWalletByPartner(Guid id)
        {
            var partner = await _partnerService.GetPartner(id);
            if (partner != null)
            {
                return SendResponse(await _service.GetWallet(id));
            }
            return SendResponse(partner);
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Wallet)]
        public async Task<IActionResult> GetWalletByAdmin()
        {
            return SendResponse(await _service.GetWallet(null));
        }
    }
}