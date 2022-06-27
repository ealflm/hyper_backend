using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class PriceOfBookingService
    {
        public PriceOfBookingService()
        {
            OrderDetailOfBookingServices = new HashSet<OrderDetailOfBookingService>();
        }

        public Guid PriceOfBookingServiceId { get; set; }
        public Guid VehicleTypeId { get; set; }
        public decimal FixedPrice { get; set; }
        public decimal FixedDistance { get; set; }
        public decimal PricePerKilometer { get; set; }
        public int Status { get; set; }

        public virtual VehicleType VehicleType { get; set; }
        public virtual ICollection<OrderDetailOfBookingService> OrderDetailOfBookingServices { get; set; }
    }
}
