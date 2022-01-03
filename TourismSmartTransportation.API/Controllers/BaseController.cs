using Microsoft.AspNetCore.Mvc;

namespace BookingYacht.API.Controllers
{
    public class BaseController : ControllerBase
    {
        protected const string Version = "v1.0";

        /*
        [NonAction]
        public virtual OkObjectResult Success(object value)
        {
            var model = new SuccessModel()
            {
                Data = value
            };
            return Ok(model);
        }

        [NonAction]
        public virtual OkObjectResult Success()
        {
            var model = new SuccessModel()
            {
                Data = "Success"
            };
            return Ok(model);
        }


        [NonAction]
        public virtual OkObjectResult Fail(string value)
        {
            var model = new ErrorModel()
            {
                Error = value
            };
            return Ok(model);
        }
        */
    }
}