using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class OrderDetailOfBookingService
    {
        public Guid OrderId { get; set; }
        public Guid PriceOfBookingServiceId { get; set; }
        public decimal Price { get; set; }
        public string Content { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }

        public virtual Order Order { get; set; }
        public virtual PriceOfBookingService PriceOfBookingService { get; set; }
    }
}
