using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Station
    {
        public Station()
        {
            RouteStations = new HashSet<RouteStation>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Status { get; set; }

        public virtual ICollection<RouteStation> RouteStations { get; set; }
    }
}
