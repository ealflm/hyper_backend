using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Station
    {
        public Station()
        {
            StationRoutes = new HashSet<StationRoute>();
        }

        public Guid StationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int Status { get; set; }

        public virtual ICollection<StationRoute> StationRoutes { get; set; }
    }
}
