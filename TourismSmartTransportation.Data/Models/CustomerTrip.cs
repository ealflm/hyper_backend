using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class CustomerTrip
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid TripId { get; set; }
        public Guid VehicleId { get; set; }
        public decimal Distance { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual Vehicle Vehicle { get; set; }
    }
}
