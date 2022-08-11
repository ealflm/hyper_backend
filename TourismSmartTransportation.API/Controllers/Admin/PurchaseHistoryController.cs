using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.Order;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.OrderDetail;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [Authorize]
    [ApiController]
    public class PurchaseHistoryController : BaseController
    {
        private readonly IPurchaseHistoryService _service;
        private readonly IOrderHelpersService _helperService;

        public PurchaseHistoryController(IPurchaseHistoryService service, IOrderHelpersService heplerService)
        {
            _service = service;
            _helperService = heplerService;
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Order + "/{customerId}")]
        public async Task<IActionResult> GetOrder(Guid customerId)
        {
            return SendResponse(await _service.GetOrder(customerId));
        }

        [HttpGet]
        [Route(ApiVer1Url.Admin.Order)]
        public async Task<IActionResult> GetAllOrder()
        {
            return SendResponse(await _service.GetOrder());
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

       /* [HttpPost]
        [Route(ApiVer1Url.Admin.Order + "/make-order-test")]
        public async Task<IActionResult> CreateOrder(MakeOrderTestModel model)
        {
            return SendResponse(await _helperService.MakeOrderTest(model));
        }*/
    }
}
