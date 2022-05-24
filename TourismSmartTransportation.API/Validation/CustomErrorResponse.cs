using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TourismSmartTransportation.Business.CommonModel;

namespace TourismSmartTransportation.API.Validation
{
    public class CustomErrorResponse
    {
        public BadRequestObjectResult ErrorResponse(ActionContext actionContext)
        {
            var errorRecordList = actionContext.ModelState
            .Where(model => model.Value.Errors.Count > 0)
            .Select(model => new Error()
            {
                ErrorMessage = model.Value.Errors.FirstOrDefault().ErrorMessage
            }).FirstOrDefault();
            return new BadRequestObjectResult(new
            {
                StatusCode = 400,
                Message = errorRecordList.ErrorMessage
            });
        }
    }

    public class Error
    {
        public string Property { get; set; }
        public string ErrorMessage { get; set; }
    }
}