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
    [Authorize]
    public class CardManagementController : BaseController
    {
        private readonly ICardManagementService _service;

        public CardManagementController(ICardManagementService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.CardMatch)]
        public async Task<IActionResult> CustomerMatch([FromBody] UpdateCardModel model)
        {
            return SendResponse(await _service.CustomerMatch(model));
        }
    }
}