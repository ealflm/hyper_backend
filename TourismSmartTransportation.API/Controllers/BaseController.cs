using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;

namespace TourismSmartTransportation.API.Controllers
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        public ObjectResult SendResponse(object result)
        {
            int statusCode = 200;
            string message = "Thành công";
            if (result == null)
            {
                statusCode = 404;
                message = "Không tìm thấy";
            }
            return HandleObjectResponse(statusCode, message, result);
        }

        [NonAction]
        public ObjectResult SendResponse(bool result)
        {
            int statusCode = result ? 201 : 400;
            string message = result ? "Thành công" : "Dữ liệu không phù hợp";
            return HandleObjectResponse(statusCode, message, null);
        }

        [NonAction]
        public ObjectResult SendResponse(Response response)
        {
            return HandleObjectResponse(response.StatusCode, response.Message, null);
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
