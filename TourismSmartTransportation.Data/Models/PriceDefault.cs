using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class PriceDefault
    {
        public PriceDefault()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public Guid Id { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid VehicleTypeId { get; set; }
        public int Seats { get; set; }
        public decimal RangeStart { get; set; }
        public decimal RangeEnd { get; set; }
        public decimal Value { get; set; }
        public int Status { get; set; }

        public virtual ServiceType ServiceType { get; set; }
        public virtual VehicleType VehicleType { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
