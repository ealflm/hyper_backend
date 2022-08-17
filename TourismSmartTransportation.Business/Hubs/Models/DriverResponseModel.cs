namespace TourismSmartTransportation.Business.Hubs.Models
{
    public class DriverResponseModel
    {
        public bool Accepted { get; set; }
        public DataHubModel Driver { get; set; }
        public DataHubModel Customer { get; set; }
    }
}