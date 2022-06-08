using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PriceRentingServiceConfig
{
    public class CreatePriceRentingServiceModel
    {
        [Required]
        public Guid PublishYearId { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        [ValidValueMinMax("MinTime", ErrorMessage = "MinTime cannot be greater than Maxtime")]
        public decimal MinTime { get; set; }
        [Required]
        public decimal MaxTime { get; set; }
        [Required]
        public decimal PricePerHour { get; set; }
        [Required]
        public decimal FixedPrice { get; set; }
        [Required]
        public decimal WeekendPrice { get; set; }
        [Required]
        public decimal HolidayPrice { get; set; }
    }
}