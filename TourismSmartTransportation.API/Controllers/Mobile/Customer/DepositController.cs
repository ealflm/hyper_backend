using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Customer.Deposit)]
    public class DepositController : BaseController
    {
        private readonly IDepositService _service;

        public DepositController(IDepositService service)
        {
            _service = service;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderStatus(string id)
        {
            return SendResponse(await _service.GetOrderStatus(id));
        }

        [HttpPost]
        // [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> GetOrderId([FromForm] DepositSearchModel model)
        {
            return SendResponse(await _service.GetOrderId(model));
        }
    }
}