using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class FeedbackForVehicle
    {
        public Guid CustomerTripId { get; set; }
        public int Rate { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public Guid FeedbackVehicleId { get; set; }

        public virtual CustomerTrip CustomerTrip { get; set; }
    }
}
