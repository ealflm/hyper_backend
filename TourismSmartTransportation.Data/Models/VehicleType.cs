using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class VehicleType
    {
        public VehicleType()
        {
            PriceBookings = new HashSet<PriceBooking>();
            Vehicles = new HashSet<Vehicle>();
        }

        public Guid Id { get; set; }
        public string Label { get; set; }
        public int Seats { get; set; }
        public string Fuel { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }

        public virtual ICollection<PriceBooking> PriceBookings { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
