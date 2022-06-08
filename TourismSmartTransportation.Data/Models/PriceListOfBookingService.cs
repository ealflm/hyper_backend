using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class PriceListOfBookingService
    {
        public PriceListOfBookingService()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public Guid Id { get; set; }
        public Guid VehicleTypeId { get; set; }
        public decimal FixedPrice { get; set; }
        public decimal FixedDistance { get; set; }
        public decimal PricePerKilometer { get; set; }
        public int Status { get; set; }

        public virtual VehicleType VehicleType { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
