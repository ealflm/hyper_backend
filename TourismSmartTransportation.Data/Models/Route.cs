using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Route
    {
        public Route()
        {
            LinkRouteFirstRoutes = new HashSet<LinkRoute>();
            LinkRouteSecondRoutes = new HashSet<LinkRoute>();
            PartnerRoutes = new HashSet<PartnerRoute>();
            RoutePriceBusings = new HashSet<RoutePriceBusing>();
            StationRoutes = new HashSet<StationRoute>();
            Trips = new HashSet<Trip>();
        }

        public Guid RouteId { get; set; }
        public Guid PartnerId { get; set; }
        public string Name { get; set; }
        public int TotalStation { get; set; }
        public decimal Distance { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Status { get; set; }

        public virtual ICollection<LinkRoute> LinkRouteFirstRoutes { get; set; }
        public virtual ICollection<LinkRoute> LinkRouteSecondRoutes { get; set; }
        public virtual ICollection<PartnerRoute> PartnerRoutes { get; set; }
        public virtual ICollection<RoutePriceBusing> RoutePriceBusings { get; set; }
        public virtual ICollection<StationRoute> StationRoutes { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
    }
}
