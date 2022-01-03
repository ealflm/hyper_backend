using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class CompanyTrip
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public Guid CompanyId { get; set; }
        public int Status { get; set; }

        public virtual Company Company { get; set; }
        public virtual Trip IdNavigation { get; set; }
    }
}
