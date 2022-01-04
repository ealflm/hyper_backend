using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.API.Utilities.Response;
using TourismSmartTransportation.Business.SearchModel.Admin.Authorization;
using TourismSmartTransportation.Business.ViewModel.Admin.Authorization;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [Route(AdminRoute)]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = Role)]
    public class AuthorizationController : BaseAdminController
    {
        private readonly Business.Interfaces.Admin.IAuthorizationService _authorizationService;

        public AuthorizationController(Business.Interfaces.Admin.IAuthorizationService authorizationService)
        {
            this._authorizationService = authorizationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginSearchModel model)
        {
            var result = await _authorizationService.Login(model);

            if (result.Data == null)
            {
                return Problem(result.Message,"Unauthorization",401);  
            }
            var loginViewModel = new LoginViewModel(result.Data.ToString());
            return Ok(loginViewModel);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterSearchModel model)
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
