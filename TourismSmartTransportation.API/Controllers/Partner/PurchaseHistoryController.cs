﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.Order;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.OrderDetail;

namespace TourismSmartTransportation.API.Controllers.Partner
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
        [Route(ApiVer1Url.Partner.Order + "/{partnerId}")]
        public async Task<IActionResult> GetOrder(Guid partnerId)
        {
            return SendResponse(await _service.GetOrderByPartnerId(partnerId));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.OrderDetail + "/{orderId}")]
        public async Task<IActionResult> GetOrderDetail(Guid orderId)
        {
            return SendResponse(await _service.GetOrderDetail(orderId));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Transaction + "/{orderId}")]
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
