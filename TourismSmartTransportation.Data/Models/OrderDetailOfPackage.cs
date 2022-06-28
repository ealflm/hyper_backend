using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class OrderDetailOfPackage
    {
        public Guid OrderId { get; set; }
        public Guid PackageId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }

        public virtual Order Order { get; set; }
        public virtual Package Package { get; set; }
    }
}
