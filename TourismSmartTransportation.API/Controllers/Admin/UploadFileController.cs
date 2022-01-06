using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Interfaces;
using TourismSmartTransportation.Business.SearchModel.Common;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.API.Controllers.Admin
{
    [ApiController]
    [Authorize]
    public class UploadFileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUploadFileService _uploadFileService;

        public UploadFileController(IConfiguration configuration, IUploadFileService uploadFileService)
        {
            _configuration = configuration;
            _uploadFileService = uploadFileService;
        }

        [Route(ApiVer1Url.Admin.UploadFile)]
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] UploadFileSearchModel model)
        {
            string fileName = null;
            if (model.ImageFile != null)
            {
                fileName= await _uploadFileService.Upload(model);
            }
            else
            {
                return ValidationProblem();
            }
            if (fileName == null)
            {
                return ValidationProblem();
            }
            var result = new UploadFileViewModel()
            {
                Link= _configuration["AzureBlob:ReturnPath"] + fileName
            };
            return Ok(result);
        }
    }
}
