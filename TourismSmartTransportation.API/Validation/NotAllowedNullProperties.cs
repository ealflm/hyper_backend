using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TourismSmartTransportation.Business.CommonModel;

namespace TourismSmartTransportation.API.Validation
{
    public class NotAllowedNullPropertiesAttribute : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var objectModel = context.ActionArguments.SingleOrDefault(o => o.Value is Object);
            if (objectModel.Value is null)
            {
                context.Result = new BadRequestObjectResult(new Response() { StatusCode = 422, Message = "Cannot empty data" });
                return;
            }

            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }

            if (objectModel.Value.GetType() != typeof(Object))
            {
                return;
            }

            foreach (var p in objectModel.Value.GetType().GetProperties())
            {
                if (p.Name == "PhotoUrls" || p.Name.Contains("File") || p.Name == "Status")
                    continue;

                if (p.GetValue(objectModel.Value) == null)
                {
                    context.Result = new BadRequestObjectResult(new Response(400, $"{p.Name} is required property"));
                }
            }
        }
    }
}