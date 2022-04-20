using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class OrderOfService
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid ServiceId { get; set; }
        public Guid OrderId { get; set; }
        public int Status { get; set; }

        public virtual Order Order { get; set; }
        public virtual Service Service { get; set; }
    }
}
