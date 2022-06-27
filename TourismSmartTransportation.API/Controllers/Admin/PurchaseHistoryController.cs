// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using TourismSmartTransportation.Business.Interfaces.Admin;

// namespace TourismSmartTransportation.API.Controllers.Admin
// {
//     [Authorize]
//     [ApiController]
//     public class PurchaseHistoryController : BaseController
//     {
//         private readonly IPurchaseHistoryService _service;

//         public PurchaseHistoryController(IPurchaseHistoryService service)
//         {
//             _service = service;
//         }

//         [HttpGet]
//         [Route(ApiVer1Url.Admin.Order)]
//         public async Task<IActionResult> GetOrder([FromQuery] string customerId)
//         {
//             return SendResponse(await _service.GetOrder(customerId));
//         }

//         [HttpGet]
//         [Route(ApiVer1Url.Admin.OrderDetail)]
//         public async Task<IActionResult> GetOrderDetail([FromQuery] string orderId)
//         {
//             return SendResponse(await _service.GetOrderDetail(orderId));
//         }

//         [HttpGet]
//         [Route(ApiVer1Url.Admin.Payment)]
//         public async Task<IActionResult> GetTransaction([FromQuery] string orderId)
//         {
//             return SendResponse(await _service.GetPayment(orderId));
//         }
//     }
// }
