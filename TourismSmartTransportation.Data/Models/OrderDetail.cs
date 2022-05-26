using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class OrderDetail
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid? TierId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public Guid? PriceBookingId { get; set; }
        public Guid? PriceRentingId { get; set; }
        public Guid? PriceBusingId { get; set; }

        public virtual Order Order { get; set; }
        public virtual PriceBooking PriceBooking { get; set; }
        public virtual PriceBusing PriceBusing { get; set; }
        public virtual PriceRenting PriceRenting { get; set; }
        public virtual Tier Tier { get; set; }
    }
}
