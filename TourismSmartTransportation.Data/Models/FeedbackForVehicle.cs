using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class FeedbackForVehicle
    {
        public Guid OrderId { get; set; }
        public Guid VehicelId { get; set; }
        public int Rate { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }

        public virtual Order Order { get; set; }
        public virtual Vehicle Vehicel { get; set; }
    }
}
