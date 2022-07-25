using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class LinkRoute
    {
        public Guid FirstRouteId { get; set; }
        public Guid SecondRouteId { get; set; }
        public Guid? StationId { get; set; }
        public Guid? LinkStationId { get; set; }

        public virtual Route FirstRoute { get; set; }
        public virtual Route SecondRoute { get; set; }
    }
}
