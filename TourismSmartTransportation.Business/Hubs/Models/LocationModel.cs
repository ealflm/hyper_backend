namespace TourismSmartTransportation.Business.Hubs.Models
{
    public class LocationModel
    {
        public string Id { get; set; } // Id of customer/driver
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string PriceBookingId { get; set; }
        public decimal Price { get; set; }
        public decimal Distance { get; set; }
        public int Seats { get; set; }
    }
}