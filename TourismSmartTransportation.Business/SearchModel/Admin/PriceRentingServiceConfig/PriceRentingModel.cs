using System;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PriceRentingServiceConfig
{
    public class PriceRentingModel
    {
        public virtual Guid? PublishYearId { get; set; }

        public virtual Guid? CategoryId { get; set; }

        [ValidValueMinMax("MinTime", ErrorMessage = "MinTime cannot be greater than Maxtime")]
        public virtual decimal? MinTime { get; set; }

        public virtual decimal? MaxTime { get; set; }

        public virtual decimal? PricePerHour { get; set; }

        public virtual decimal? FixedPrice { get; set; }

        public virtual decimal? WeekendPrice { get; set; }

        public virtual decimal? HolidayPrice { get; set; }
    }
}