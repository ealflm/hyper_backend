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

        public PurchaseHistoryController(IPurchaseHistoryService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseHistory(Guid id)
        {
            return SendResponse(await _service.GetPurchase(id));
        }
    }
}