using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class BasePriceOfBusService
    {
        public BasePriceOfBusService()
        {
            PriceOfBusServices = new HashSet<PriceOfBusService>();
        }

        public Guid BasePriceOfBusServiceId { get; set; }
        public decimal MaxDistance { get; set; }
        public decimal MinDistance { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }

        public virtual ICollection<PriceOfBusService> PriceOfBusServices { get; set; }
    }
}
