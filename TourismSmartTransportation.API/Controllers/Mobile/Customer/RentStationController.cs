using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.Interfaces.Company;
using TourismSmartTransportation.Business.SearchModel.Admin.Package;
using TourismSmartTransportation.Business.SearchModel.Partner.RentStationManagement;

namespace TourismSmartTransportation.API.Controllers.Mobile.Customer
{
    [ApiController]
    [Authorize]
    public class RentStationController : BaseController
    {
        private readonly IRentStationManagementService _service;

        public RentStationController(IRentStationManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(ApiVer1Url.Customer.RentStation + "/list")]
        public async Task<IActionResult> SearchPackage([FromQuery] RentStationSearchModel model)
        {
            return SendResponse(await _service.SearchRentStation(model));
        }
    }
}