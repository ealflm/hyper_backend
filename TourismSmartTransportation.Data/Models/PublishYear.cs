using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class PublishYear
    {
        public PublishYear()
        {
            PriceRentings = new HashSet<PriceRenting>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }

        public virtual ICollection<PriceRenting> PriceRentings { get; set; }
    }
}
