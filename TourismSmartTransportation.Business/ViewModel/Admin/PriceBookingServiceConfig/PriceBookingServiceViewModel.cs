using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.PriceBookingServiceViewModel
{
    public class PriceBookingServiceViewModel
    {
        public Guid Id { get; set; }
        public Guid VehicleTypeId { get; set; }
        public decimal FixedPrice { get; set; }
        public decimal FixedDistance { get; set; }
        public decimal PricePerKilometer { get; set; }
        public int Status { get; set; }
    }
}