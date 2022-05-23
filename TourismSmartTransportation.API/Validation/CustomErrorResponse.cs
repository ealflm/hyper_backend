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
                Property = model.Key,
                ErrorMessage = model.Value.Errors.FirstOrDefault().ErrorMessage
            }).ToList();
            return new BadRequestObjectResult(new
            {
                StatusCode = 400,
                Message = errorRecordList
            });
        }
    }

    public class Error
    {
        public string Property { get; set; }
        public string ErrorMessage { get; set; }
    }
}