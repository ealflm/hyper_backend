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
        public int MaxDistance { get; set; }
        public int MinDistance { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }

        public virtual ICollection<PriceOfBusService> PriceOfBusServices { get; set; }
    }
}
