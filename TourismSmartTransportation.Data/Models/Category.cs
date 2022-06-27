using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Category
    {
        public Category()
        {
            PriceOfRentingServices = new HashSet<PriceOfRentingService>();
        }

        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }

        public virtual ICollection<PriceOfRentingService> PriceOfRentingServices { get; set; }
    }
}
