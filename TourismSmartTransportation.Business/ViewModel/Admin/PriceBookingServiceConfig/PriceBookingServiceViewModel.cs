using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.PriceBookingServiceViewModel
{
    public class PriceOfBookingServiceViewModel
    {
        public Guid Id { get; set; }
        public Guid VehicleTypeId { get; set; }
        public string VehicleName { get; set; }
        public decimal FixedPrice { get; set; }
        public decimal FixedDistance { get; set; }
        public decimal PricePerKilometer { get; set; }
        public int Status { get; set; }
    }
}