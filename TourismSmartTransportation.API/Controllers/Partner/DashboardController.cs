using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.SearchModel.Partner.Dashboard;
using TourismSmartTransportation.Business.SearchModel.Partner.Route;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TourismSmartTransportation.API.Controllers.Partner
{
    [ApiController]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Dashboard + "/driver")]
        public async Task<IActionResult> GetDriversList([FromQuery] ReportSearchModel model)
        {
            return SendResponse(await _service.GetDriverReportByMonth(model));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Dashboard + "/trip")]
        public async Task<IActionResult> GetTripsList([FromQuery] ReportSearchModel model)
        {
            return SendResponse(await _service.GetTripReportByMonth(model));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Dashboard + "/revenue")]
        public async Task<IActionResult> GetRevenueReportByMonth([FromQuery] ReportSearchModel model)
        {
            return SendResponse(await _service.GetRevenueReportByMonth(model));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Dashboard + "/statistic")]
        public async Task<IActionResult> GetStatisticalReport([FromQuery] ReportSearchModel model)
        {
            return SendResponse(await _service.GetStatisticalReport(model));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.Dashboard + "/vehicle-service-type")]
        public async Task<IActionResult> GetVehicleReportByServiceType([FromQuery] ReportSearchModel model)
        {
            return SendResponse(await _service.GetVehicleReportByServiceType(model));
        }
    }
}
