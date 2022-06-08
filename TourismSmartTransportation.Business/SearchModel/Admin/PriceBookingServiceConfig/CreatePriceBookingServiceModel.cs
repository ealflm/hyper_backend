using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PriceBookingServiceConfig
{
    public class CreatePriceBookingServiceModel
    {
        [Required]
        public Guid VehicleTypeId { get; set; }
        [Required]
        public decimal FixedPrice { get; set; }
        [Required]
        public decimal FixedDistance { get; set; }
        [Required]
        public decimal PricePerKilometer { get; set; }
    }
}