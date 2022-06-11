using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Trip
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public Guid PartnerId { get; set; }
        public Guid VehicleId { get; set; }
        public Guid RouteId { get; set; }
        public string TripName { get; set; }
        public int DayOfWeek { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public int Status { get; set; }

        public virtual Driver Driver { get; set; }
        public virtual Partner Partner { get; set; }
        public virtual Route Route { get; set; }
        public virtual Vehicle Vehicle { get; set; }
    }
}
