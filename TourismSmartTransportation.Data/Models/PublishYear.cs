using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class PublishYear
    {
        public PublishYear()
        {
            PriceListOfRentingServices = new HashSet<PriceListOfRentingService>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }

        public virtual ICollection<PriceListOfRentingService> PriceListOfRentingServices { get; set; }
    }
}
