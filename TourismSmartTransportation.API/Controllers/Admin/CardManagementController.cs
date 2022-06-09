using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.API.Validation;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.CardManagement;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [ApiController]
    [Authorize]
    [Route(ApiVer1Url.Admin.Card)]
    public class CardManagementController : BaseController
    {
        private readonly ICardManagementService _service;

        public CardManagementController(ICardManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] CardSearchModel model)
        {
            return SendResponse(await _service.Search(model));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return SendResponse(await _service.GetById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] string uid)
        {
            return SendResponse(await _service.Create(uid));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCardModel model)
        {
            return SendResponse(await _service.Update(id, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return SendResponse(await _service.Delete(id));
        }
    }
}