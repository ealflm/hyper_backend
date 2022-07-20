using System;

namespace TourismSmartTransportation.Business.ViewModel.Mobile.Customer
{
    public class CustomerTripViewModel
    {
        public Guid CustomerId { get; set; }
        public Guid RouteId { get; set; }
        public Guid VehicleId { get; set; }
        public decimal Distance { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public decimal? LongitudeOfPickUpPoint { get; set; }
        public decimal? LatitudeOfPickUpPoint { get; set; }
        public decimal? LongitudeOfDestination { get; set; }
        public decimal? LatitudeOfDestination { get; set; }
        public int Status { get; set; }
    }
}