using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PriceRentingServiceConfig
{
    public class CreatePriceRentingServiceModel : PriceRentingModel
    {
        [Required]
        public override Guid? PublishYearId { get; set; }
        [Required]
        public override Guid? CategoryId { get; set; }
        [Required]
        public override decimal? MinTime { get; set; }
        [Required]
        public override decimal? MaxTime { get; set; }
        [Required]
        public override decimal? PricePerHour { get; set; }
        [Required]
        public override decimal? FixedPrice { get; set; }
        [Required]
        public override decimal? WeekendPrice { get; set; }
        [Required]
        public override decimal? HolidayPrice { get; set; }
    }
}