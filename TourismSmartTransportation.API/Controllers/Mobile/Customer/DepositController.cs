using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [ApiController]
    [Route(ApiVer1Url.Customer.Deposit)]
    public class DepositController : BaseController
    {
        private readonly IDepositService _service;

        public DepositController(IDepositService service)
        {
            _service = service;
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderStatus(string id)
        {
            return SendResponse(await _service.GetOrderStatus(id));
        }

        [HttpPost]
        [Authorize]
        // [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> GetOrderId([FromForm] DepositSearchModel model)
        {
            return SendResponse(await _service.GetOrderId(model));
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.DepositMoMo)]
        // [ServiceFilter(typeof(NotAllowedNullPropertiesAttribute))]
        public async Task<IActionResult> GetOrderMoMoStatus()
        {
            JObject jmessage = JObject.Parse(Response.Body.ToString());
            Guid id = new Guid(jmessage.GetValue("orderId").ToString());
            int status = int.Parse(jmessage.GetValue("resultCode").ToString());
            return SendResponse(await _service.GetOrderMoMoStatus(id, status));
        }


    }
}