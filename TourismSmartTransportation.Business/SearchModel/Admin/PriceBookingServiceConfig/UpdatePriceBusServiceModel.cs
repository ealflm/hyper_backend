using System;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PriceBookingServiceConfig
{
    public class UpdatePriceBookingServiceModel
    {
        public Guid? VehicleTypeId { get; set; }
        public decimal? FixedPrice { get; set; }
        public decimal? FixedDistance { get; set; }
        public decimal? PricePerKilometer { get; set; }
        public int? Status { get; set; }
    }
}