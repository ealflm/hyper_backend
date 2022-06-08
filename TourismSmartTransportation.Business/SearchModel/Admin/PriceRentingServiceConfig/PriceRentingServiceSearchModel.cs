using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PriceRentingServiceConfig
{
    public class PriceRentingServiceSearchModel
    {
        public Guid? PublishYearId { get; set; }
        public Guid? CategoryId { get; set; }
        [ValidValueMinMax("MinTime", ErrorMessage = "MinTime cannot be greater than Maxtime")]
        public decimal? MinTime { get; set; }
        public decimal? MaxTime { get; set; }
        public decimal? PricePerHour { get; set; }
        public decimal? FixedPrice { get; set; }
        public decimal? WeekendPrice { get; set; }
        public decimal? HolidayPrice { get; set; }
        public int? Status { get; set; }
    }
}