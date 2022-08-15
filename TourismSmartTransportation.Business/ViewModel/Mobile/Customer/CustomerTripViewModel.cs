using System;

namespace TourismSmartTransportation.Business.ViewModel.Mobile.Customer
{
    public class CustomerTripViewModel
    {
        public Guid CustomerTripId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Guid? TripId { get; set; }
        public Guid VehicleId { get; set; }
        public decimal? Distance { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime? RentDeadline { get; set; }
        public string Coordinates { get; set; }
        public string ServiceTypeName { get; set; }
        public string VehicleName { get; set; }
        public string LicensePlates { get; set; }
        // public decimal? LongitudeOfPickUpPoint { get; set; }
        // public decimal? LatitudeOfPickUpPoint { get; set; }
        // public decimal? LongitudeOfDestination { get; set; }
        // public decimal? LatitudeOfDestination { get; set; }
        public int Status { get; set; }
    }
}