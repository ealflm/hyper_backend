using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class TripActivity
    {
        public Guid Id { get; set; }
        public Guid OrderDetailId { get; set; }
        public string Path { get; set; }
        public int Status { get; set; }

        public virtual OrderDetail OrderDetail { get; set; }
    }
}
