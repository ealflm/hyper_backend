using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [ApiController]
    [Route(ApiVer1Url.Customer.PurchaseHistory)]
    [Authorize]
    public class PurchaseHistoryController : BaseController
    {
        private readonly IPurchaseHistoryService _service;
        private readonly TourismSmartTransportation.Business.Interfaces.Admin.IPurchaseHistoryService _purchase;
        public PurchaseHistoryController(IPurchaseHistoryService service, TourismSmartTransportation.Business.Interfaces.Admin.IPurchaseHistoryService purchase)
        {
            _service = service;
            _purchase = purchase;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseHistory(Guid id)
        {
            return SendResponse(await _service.GetPurchase(id));
        }

        [HttpGet("order-detail/{id}")]
        public async Task<IActionResult> GetOrderDetail(Guid id)
        {
            return SendResponse(await _purchase.GetOrderDetail(id));
        }
    }
}