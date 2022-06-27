using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class FeedbackForDriver
    {
        public Guid OrderId { get; set; }
        public Guid DriverId { get; set; }
        public int Rate { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }

        public virtual Driver Driver { get; set; }
        public virtual Order Order { get; set; }
    }
}
