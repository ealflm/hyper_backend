using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Trip
    {
        public Trip()
        {
            Rents = new HashSet<Rent>();
            Vehicles = new HashSet<Vehicle>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid RouteId { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public int Status { get; set; }

        public virtual Route Route { get; set; }
        public virtual CompanyTrip CompanyTrip { get; set; }
        public virtual ICollection<Rent> Rents { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
