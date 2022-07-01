using System;
using System.ComponentModel.DataAnnotations;

namespace TourismSmartTransportation.Business.SearchModel.Partner.Route
{
    public class CreateTripModel : TripModel
    {
        [Required]
        public override Guid? RouteId { get; set; }

        [Required]
        public override Guid? VehicleId { get; set; }

        [Required]
        public override Guid? DriverId { get; set; }

        [Required]
        public override string TripName { get; set; }

        [Required]
        public override int? DayOfWeek { get; set; }

        [Required]
        public override DateTime? TimeStart { get; set; }

        [Required]
        public override DateTime? TimeEnd { get; set; }
    }
}