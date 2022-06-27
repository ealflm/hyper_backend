using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class RoutePriceBusing
    {
        public Guid RouteId { get; set; }
        public Guid PriceBusingId { get; set; }

        public virtual PriceOfBusService PriceBusing { get; set; }
        public virtual Route Route { get; set; }
    }
}
