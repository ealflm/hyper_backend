using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class ServiceDetail
    {
        public ServiceDetail()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid DiscountId { get; set; }
        public int Status { get; set; }

        public virtual Discount Discount { get; set; }
        public virtual Service Service { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
