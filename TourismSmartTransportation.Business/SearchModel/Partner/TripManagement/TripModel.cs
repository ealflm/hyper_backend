using System;

namespace TourismSmartTransportation.Business.SearchModel.Partner.Route
{
    public class TripModel
    {
        public virtual Guid? RouteId { get; set; }

        public virtual Guid? VehicleId { get; set; }

        public virtual Guid? DriverId { get; set; }

        public virtual string TripName { get; set; }

        public virtual int? DayOfWeek { get; set; }

        public virtual DateTime? TimeStart { get; set; }

        public virtual DateTime? TimeEnd { get; set; }
    }
}