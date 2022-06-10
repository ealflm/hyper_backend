using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PriceBookingServiceConfig
{
    public class PriceBookingServiceSearchModel
    {
        
        public Guid? VehicleTypeId { get; set; }
        public decimal? FixedPrice { get; set; }
        public decimal? FixedDistance { get; set; }
        public decimal? PricePerKilometer { get; set; }
        public int? Status { get; set; }
    }
}