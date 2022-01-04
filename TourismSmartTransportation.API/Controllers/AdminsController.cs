using BookingYacht.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces;

namespace TourismSmartTransportation.API.Controllers
{
    [Route("api/" + Version + "/admins")]
    [ApiController]
    [Authorize]
    public class AdminsController : BaseController
    {
        private readonly IAdminService _service;

        public AdminsController(IAdminService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var admins = await _service.GetAdmin();
            return Ok(admins);
        }
    }
}
