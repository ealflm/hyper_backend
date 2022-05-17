using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Tier
    {
        public Tier()
        {
            CustomerTierHistories = new HashSet<CustomerTierHistory>();
            Customers = new HashSet<Customer>();
            OrderDetails = new HashSet<OrderDetail>();
            Packages = new HashSet<Package>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PromotedTitle { get; set; }
        public int Status { get; set; }

        public virtual ICollection<CustomerTierHistory> CustomerTierHistories { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Package> Packages { get; set; }
    }
}
