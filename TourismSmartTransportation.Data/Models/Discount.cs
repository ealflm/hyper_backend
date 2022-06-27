using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Discount
    {
        public Discount()
        {
            Orders = new HashSet<Order>();
        }

        public Guid DiscountId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string PhotoUrl { get; set; }
        public decimal Value { get; set; }
        public int Status { get; set; }

        public virtual ServiceType ServiceType { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
