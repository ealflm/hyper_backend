namespace TourismSmartTransportation.Business.Hubs.Models
{
    public class DriverResponseModel
    {
        public int StatusCode { get; set; }
        public DataHubModel Driver { get; set; }
        public DataHubModel Customer { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }
}