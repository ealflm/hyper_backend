using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class ServiceDetail
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public int Status { get; set; }

        public virtual Service Service { get; set; }
        public virtual ServiceType ServiceType { get; set; }
    }
}
