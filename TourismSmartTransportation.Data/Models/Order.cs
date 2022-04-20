using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            OrderOfServices = new HashSet<OrderOfService>();
            Payments = new HashSet<Payment>();
        }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<OrderOfService> OrderOfServices { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
