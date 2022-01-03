using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class ServiceDiscount
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public Guid DiscountId { get; set; }
        public int Status { get; set; }

        public virtual Discount Discount { get; set; }
        public virtual Service Service { get; set; }
    }
}
