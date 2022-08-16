namespace TourismSmartTransportation.Business.Hubs
{
    public class DataHubModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoUrl { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public int Status { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string PlaceName { get; set; }
        public string Address { get; set; }
        public double DistanceBetween { get; set; } // Khoảng cách giữa khách hàng và tài xế.
        public int IntervalLoopIndex { get; set; } = 0;
        public int ItemIndex { get; set; } = 0;
        public string PriceBookingId { get; set; }
        public decimal Price { get; set; }
        public decimal Distance { get; set; } // khoảng cách tuyến khách hàng đặt
        public int Seats { get; set; }
    }
}