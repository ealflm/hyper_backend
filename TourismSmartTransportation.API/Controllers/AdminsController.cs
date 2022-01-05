using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces;

namespace TourismSmartTransportation.API.Controllers
{
    [Route("api/v1.0/admin/admins")]
    [ApiController]
    [Authorize]
    public class AdminsController : ControllerBase
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
