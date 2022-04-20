using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class RouteStation
    {
        public Guid Id { get; set; }
        public Guid RouteId { get; set; }
        public Guid StationId { get; set; }
        public int Station { get; set; }

        public virtual Route Route { get; set; }
        public virtual Station StationNavigation { get; set; }
    }
}
