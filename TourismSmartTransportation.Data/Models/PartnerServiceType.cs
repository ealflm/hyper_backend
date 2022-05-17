using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class PartnerServiceType
    {
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public int Status { get; set; }

        public virtual Partner Partner { get; set; }
        public virtual ServiceType ServiceType { get; set; }
    }
}
