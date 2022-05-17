using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class OrderDetail
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid TierId { get; set; }
        public Guid PriceDefaultId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }

        public virtual Order Order { get; set; }
        public virtual PriceDefault PriceDefault { get; set; }
        public virtual Tier Tier { get; set; }
    }
}
