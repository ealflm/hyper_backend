namespace TourismSmartTransportation.Business.CommonModel
{
    public class DataModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public DataModel(int statusCode, object data, string message)
        {
            StatusCode = statusCode;
            Data = data;
            Message = message;
        }

    }
}