using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class StationRoute
    {
        public int OrderNumber { get; set; }
        public Guid StationId { get; set; }
        public Guid RouteId { get; set; }
        public int Status { get; set; }
        public decimal Distance { get; set; }

        public virtual Route Route { get; set; }
        public virtual Station Station { get; set; }
    }
}
