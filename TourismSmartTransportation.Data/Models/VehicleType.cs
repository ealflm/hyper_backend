using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class VehicleType
    {
        public VehicleType()
        {
            PriceOfBookingServices = new HashSet<PriceOfBookingService>();
            Vehicles = new HashSet<Vehicle>();
        }

        public Guid VehicleTypeId { get; set; }
        public string Label { get; set; }
        public int Seats { get; set; }
        public string Fuel { get; set; }
        public int Status { get; set; }

        public virtual ICollection<PriceOfBookingService> PriceOfBookingServices { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
