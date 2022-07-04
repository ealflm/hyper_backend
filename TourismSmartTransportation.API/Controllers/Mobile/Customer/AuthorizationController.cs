using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.API.Utilities.Response;
using TourismSmartTransportation.Business.SearchModel.Common.Authorization;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer.Authorization;
using TourismSmartTransportation.Business.ViewModel.Admin.Authorization;

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [ApiController]
    public class AuthorizationController : BaseController
    {
        private readonly Business.Interfaces.IAuthorizationService _authorizationService;

        public AuthorizationController(Business.Interfaces.IAuthorizationService authorizationService)
        {
            this._authorizationService = authorizationService;
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.Login)]
        public async Task<IActionResult> Login([FromBody] LoginSearchModel model)
        {
            var result = await _authorizationService.Login(model, global::Login.Customer);

            if (result.Data == null)
            {
                return Problem(result.Message, "Unauthorization", 401);
            }
            var loginViewModel = new LoginViewModel(result.Data.ToString());
            return Ok(loginViewModel);
        }

        [HttpPost]
        [Route(ApiVer1Url.Customer.Register)]
        public async Task<IActionResult> RegisterForCustomer([FromForm] RegisterModel model)
        {
            return SendResponse(await _authorizationService.RegisterForCustomer(model));
        }

    }
}
