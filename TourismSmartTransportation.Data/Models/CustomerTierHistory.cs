using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class CustomerTierHistory
    {
        public CustomerTierHistory()
        {
            PackageStatuses = new HashSet<PackageStatus>();
        }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid TierId { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public int Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Tier Tier { get; set; }
        public virtual ICollection<PackageStatus> PackageStatuses { get; set; }
    }
}
