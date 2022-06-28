using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class ServiceType
    {
        public ServiceType()
        {
            Discounts = new HashSet<Discount>();
            Orders = new HashSet<Order>();
            PackageItems = new HashSet<PackageItem>();
            PartnerServiceTypes = new HashSet<PartnerServiceType>();
            Vehicles = new HashSet<Vehicle>();
        }

        public Guid ServiceTypeId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Discount> Discounts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<PackageItem> PackageItems { get; set; }
        public virtual ICollection<PartnerServiceType> PartnerServiceTypes { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
