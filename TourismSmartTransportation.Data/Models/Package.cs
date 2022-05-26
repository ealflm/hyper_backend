using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Package
    {
        public Guid Id { get; set; }
        public Guid TierId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public string Name { get; set; }
        public decimal Limit { get; set; }
        public decimal Value { get; set; }
        public int Status { get; set; }

        public virtual ServiceType ServiceType { get; set; }
        public virtual Tier Tier { get; set; }
    }
}
