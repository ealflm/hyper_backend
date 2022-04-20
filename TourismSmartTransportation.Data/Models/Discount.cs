using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Discount
    {
        public Discount()
        {
            ServiceDetails = new HashSet<ServiceDetail>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string PhotoUrls { get; set; }
        public decimal Value { get; set; }
        public int Status { get; set; }

        public virtual ICollection<ServiceDetail> ServiceDetails { get; set; }
    }
}
