using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.CategoryManagement;
using TourismSmartTransportation.Business.SearchModel.Admin.PublishYearManagement;
using TourismSmartTransportation.Business.SearchModel.Admin.VehicleType;

namespace TourismSmartTransportation.API.Controllers.Partner
{
    [ApiController]
    public class ConfigurationManagementController : BaseController
    {
        private readonly IVehicleTypeService _vehicleTypeService;
        private readonly IPublishYearManagementService _publishYearService;
        private readonly ICategoryManagementService _categoryService;

        public ConfigurationManagementController(IVehicleTypeService vehicleTypeService, IPublishYearManagementService publishYearService,
                                                                                                    ICategoryManagementService categoryService)
        {
            _vehicleTypeService = vehicleTypeService;
            _publishYearService = publishYearService;
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.GetConfiguration + "/vehicle-types")]
        public async Task<IActionResult> GetVehicleTypesList([FromQuery] VehicleTypeSearchModel model)
        {
            return SendResponse(await _vehicleTypeService.GetListVehicleTypes(model));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.GetConfiguration + "/publish-years")]
        public async Task<IActionResult> GetPublishYearsList([FromQuery] PublishYearSearchModel model)
        {
            return SendResponse(await _publishYearService.GetAll(model));
        }

        [HttpGet]
        [Route(ApiVer1Url.Partner.GetConfiguration + "/categories")]
        public async Task<IActionResult> GetCategories([FromQuery] CategorySearchModel model)
        {
            return SendResponse(await _categoryService.GetAll(model));
        }
    }
}