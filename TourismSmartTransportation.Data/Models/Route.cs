using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Route
    {
        public Route()
        {
            StationRoutes = new HashSet<StationRoute>();
            Trips = new HashSet<Trip>();
        }

        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public string Name { get; set; }
        public decimal Distance { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Status { get; set; }

        public virtual Partner Partner { get; set; }
        public virtual ICollection<StationRoute> StationRoutes { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
    }
}
