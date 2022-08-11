using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class PartnerRoute
    {
        public Guid PartnerId { get; set; }
        public Guid RouteId { get; set; }

        public virtual Partner Partner { get; set; }
        public virtual Route Route { get; set; }
    }
}
