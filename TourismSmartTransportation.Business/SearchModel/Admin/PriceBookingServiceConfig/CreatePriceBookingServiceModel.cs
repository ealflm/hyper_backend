using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PriceBookingServiceConfig
{
    public class CreatePriceBookingServiceModel : PriceBookingModel
    {
        [Required]
        public override Guid? VehicleTypeId { get; set; }
        [Required]
        public override decimal? FixedPrice { get; set; }
        [Required]
        public override decimal? FixedDistance { get; set; }
        [Required]
        public override decimal? PricePerKilometer { get; set; }
    }
}