using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Service
    {
        public Service()
        {
            ServiceDetails = new HashSet<ServiceDetail>();
            ServiceDiscounts = new HashSet<ServiceDiscount>();
            WalletServices = new HashSet<WalletService>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime Time { get; set; }
        public string PhotoUrls { get; set; }
        public int Status { get; set; }

        public virtual ICollection<ServiceDetail> ServiceDetails { get; set; }
        public virtual ICollection<ServiceDiscount> ServiceDiscounts { get; set; }
        public virtual ICollection<WalletService> WalletServices { get; set; }
    }
}
