using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Admin;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [Authorize]
    [ApiController]
    public class PurchaseHistoryController : BaseController
    {
        private readonly IPurchaseHistoryService _service;

        public PurchaseHistoryController(IPurchaseHistoryService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Order + "/{customerId}")]
        public async Task<IActionResult> GetOrder(Guid customerId)
        {
            return SendResponse(await _service.GetOrder(customerId));
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.OrderDetail + "/{orderId}")]
        public async Task<IActionResult> GetOrderDetail(Guid orderId)
        {
            return SendResponse(await _service.GetOrderDetail(orderId));
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Transaction + "/{orderId}")]
        public async Task<IActionResult> GetTransaction(Guid orderId)
        {
            return SendResponse(await _service.GetTransaction(orderId));
        }
    }
}
