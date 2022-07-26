using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class FeedbackForDriver
    {
        public Guid FeedbackForDriverId { get; set; }
        public Guid CustomerTripId { get; set; }
        public Guid DriverId { get; set; }
        public int Rate { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }

        public virtual CustomerTrip CustomerTrip { get; set; }
        public virtual Driver Driver { get; set; }
    }
}
