using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class PriceBusing
    {
        public PriceBusing()
        {
            OrderDetails = new HashSet<OrderDetail>();
            RoutePriceBusings = new HashSet<RoutePriceBusing>();
        }

        public Guid Id { get; set; }
        public decimal MinRouteDistance { get; set; }
        public decimal MaxRouteDistance { get; set; }
        public decimal MinDistance { get; set; }
        public decimal MaxDistance { get; set; }
        public decimal Price { get; set; }
        public decimal MinStation { get; set; }
        public decimal MaxStation { get; set; }
        public string Mode { get; set; }
        public int Status { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<RoutePriceBusing> RoutePriceBusings { get; set; }
    }
}
