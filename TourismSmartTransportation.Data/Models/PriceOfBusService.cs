using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class PriceOfBusService
    {
        public PriceOfBusService()
        {
            OrderDetailOfBusServices = new HashSet<OrderDetailOfBusService>();
            RoutePriceBusings = new HashSet<RoutePriceBusing>();
        }

        public Guid PriceOfBusServiceId { get; set; }
        public decimal MinDistance { get; set; }
        public decimal MaxDistance { get; set; }
        public decimal Price { get; set; }
        public decimal MinStation { get; set; }
        public decimal MaxStation { get; set; }
        public string Mode { get; set; }
        public int Status { get; set; }
        public Guid BasePriceId { get; set; }

        public virtual BasePriceOfBusService BasePrice { get; set; }
        public virtual ICollection<OrderDetailOfBusService> OrderDetailOfBusServices { get; set; }
        public virtual ICollection<RoutePriceBusing> RoutePriceBusings { get; set; }
    }
}
