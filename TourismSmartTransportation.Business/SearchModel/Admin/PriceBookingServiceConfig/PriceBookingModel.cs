using System;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PriceBookingServiceConfig
{
    public class PriceBookingModel
    {
        public virtual Guid? VehicleTypeId { get; set; }
        public virtual decimal? FixedPrice { get; set; }
        public virtual decimal? FixedDistance { get; set; }
        public virtual decimal? PricePerKilometer { get; set; }

    }
}