using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class RoutePriceBusing
    {
        public Guid Id { get; set; }
        public Guid RouteId { get; set; }
        public Guid PriceBusingId { get; set; }

        public virtual PriceListOfBusService PriceBusing { get; set; }
        public virtual Route Route { get; set; }
    }
}
