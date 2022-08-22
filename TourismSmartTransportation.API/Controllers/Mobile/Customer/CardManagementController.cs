using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.CardManagement;

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [ApiController]
    [Route(ApiVer1Url.Customer.CardMatch)]
    [Authorize]
    public class CardManagementController : BaseController
    {
        private readonly ICardManagementService _service;

        public CardManagementController(ICardManagementService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CustomerMatch([FromBody] UpdateCardModel model)
        {
            return SendResponse(await _service.CustomerMatch(model));
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCard(Guid customerId)
        {
            return SendResponse(await _service.GetByCustomerId(customerId));
        }
    }
}