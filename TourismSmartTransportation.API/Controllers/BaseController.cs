using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourismSmartTransportation.API.Controllers
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        public ObjectResult SendResponse(object result)
        {
            int statusCode = 200;
            string message = "Success";
            return HandleObjectResponse(statusCode, message, result);
        }

        [NonAction]
        public ObjectResult SendResponse(bool result)
        {
            int statusCode = 201;
            string message = result ? "Success" : "Validation Problem";
            return HandleObjectResponse(statusCode, message, null);
        }

        private ObjectResult HandleObjectResponse(int statusCode, string message, object result)
        {
            ObjectResult objectResult = null;
            Object responseData = null;

            if
            (result == null)
            {
                responseData = new
                {
                    statusCode = statusCode,
                    message = message
                };
            }
            else
            {
                responseData = new
                {
                    statusCode = statusCode,
                    message = message,
                    body = result
                };
            }

            objectResult = new ObjectResult(responseData);
            objectResult.StatusCode = statusCode;
            return objectResult;
        }
    }
}
