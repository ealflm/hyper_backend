using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Route
    {
        public Route()
        {
            RouteStations = new HashSet<RouteStation>();
            Trips = new HashSet<Trip>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public decimal Distance { get; set; }
        public int Status { get; set; }

        public virtual ICollection<RouteStation> RouteStations { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
    }
}
