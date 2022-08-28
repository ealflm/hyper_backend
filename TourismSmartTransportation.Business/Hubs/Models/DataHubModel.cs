namespace TourismSmartTransportation.Business.Hubs.Models
{
    public class DataHubModel
    {
        // Thuộc tính chung
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoUrl { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public int Status { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double EndLongitude { get; set; }
        public double EndLatitude { get; set; }
        public string PlaceName { get; set; }
        public string Address { get; set; }


        // Thuộc tính của driver
        public double DistanceBetween { get; set; } // Khoảng cách giữa khách hàng và tài xế.
        public double Point { get; set; } // Điểm số của driver, hệ thống sẽ dựa trên điểm số này để tìm kiếm và phân bổ các yêu cầu đặt xe cho tài xế
        public double FeedbackPoint { get; set; }
        public string VehicleName { get; set; }
        public string LicensePlates { get; set; }


        // Thuộc tính của customer
        public int IntervalLoopIndex { get; set; } = 0; // index để tìm phạm vi tìm kiếm trong một mảng phạm vi đã qui định
        public string PriceBookingId { get; set; }
        public decimal Price { get; set; } = 0; // giá của tuyến mà khách hàng request booking
        public decimal Distance { get; set; } = 0; // khoảng cách tuyến khách hàng đặt
        public int Seats { get; set; } = 1; // Số chỗ ngồi mà customer request booking  (1 chỗ là xe máy)
        public int TotalDriverItems { get; set; } = 0;
    }
}