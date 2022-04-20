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
        public  ObjectResult SendReponse(object model)
        {
            int statusCode = 200;
            if (model == null)
            {
                statusCode = 404;
                model = new {message= "Not Found" };
            }
            ObjectResult objectResult = new ObjectResult(model);
            objectResult.StatusCode=statusCode;
            return objectResult;
        }

        [NonAction]
        public ObjectResult SendReponse(bool result)
        {
            int statusCode = 201;
            String message = "Successful";
            if (!result)
            {
                statusCode = 400;
                message = "Validation Problem";
            }
            ObjectResult objectResult = new ObjectResult(new {message= message });
            objectResult.StatusCode = statusCode;
            return objectResult;
        }
    }
}
