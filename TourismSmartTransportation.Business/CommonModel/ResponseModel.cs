namespace TourismSmartTransportation.Business.CommonModel
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public Response(int statusCode, object data, string message)
        {
            StatusCode = statusCode;
            Data = data;
            Message = message;
        }

        public Response(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public Response(int statusCode)
        {
            StatusCode = statusCode;
        }

        public Response() { }

    }
}