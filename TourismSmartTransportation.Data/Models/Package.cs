using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Package
    {
        public Package()
        {
            OrderDetailOfPackages = new HashSet<OrderDetailOfPackage>();
            PackageItems = new HashSet<PackageItem>();
        }

        public Guid PackageId { get; set; }
        public string Name { get; set; }
        public int PeopleQuanitty { get; set; }
        public int Duration { get; set; }
        public string PhotoUrl { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PromotedTitle { get; set; }
        public int Status { get; set; }

        public virtual ICollection<OrderDetailOfPackage> OrderDetailOfPackages { get; set; }
        public virtual ICollection<PackageItem> PackageItems { get; set; }
    }
}
