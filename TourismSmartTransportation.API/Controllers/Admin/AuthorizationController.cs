using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.API.Utilities.Response;
using TourismSmartTransportation.Business.SearchModel.Admin.Authorization;
using TourismSmartTransportation.Business.SearchModel.Common.Authorization;
using TourismSmartTransportation.Business.ViewModel.Admin.Authorization;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly Business.Interfaces.IAuthorizationService _authorizationService;

        public AuthorizationController(Business.Interfaces.IAuthorizationService authorizationService)
        {
            this._authorizationService = authorizationService;
        }

        [HttpPost]
        [Route(ApiVer1Url.Admin.Login)]
        public async Task<IActionResult> Login([FromBody] LoginSearchModel model)
        {
            var result = await _authorizationService.Login(model, global::Login.Admin);

            if (result.Data == null)
            {
                return Problem(result.Message, "Unauthorization", 401);
            }
            var loginViewModel = new LoginViewModel(result.Data.ToString());
            return Ok(loginViewModel);
        }

        [HttpPost]
        [Route(ApiVer1Url.Admin.Register)]
        public async Task<IActionResult> Register([FromForm] RegisterSearchModel model)
        {
            var result = await _authorizationService.Register(model);

            if (result != null)
            {
                return ValidationProblem(result.Message);
            }

            return Ok();
        }
    }
}
