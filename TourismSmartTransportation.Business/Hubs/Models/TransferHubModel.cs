namespace TourismSmartTransportation.Business.Hubs.Models
{
    public class TransferHubModel
    {
        public string Method { get; set; }
        public int StatusCode { get; set; }
        public object Driver { get; set; }
        public object Customer { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
    }
}