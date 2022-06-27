using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class VehicleType
    {
        public VehicleType()
        {
            PriceListOfBookingServices = new HashSet<PriceListOfBookingService>();
            Vehicles = new HashSet<Vehicle>();
        }

        public Guid Id { get; set; }
        public string Label { get; set; }
        public int Seats { get; set; }
        public string Fuel { get; set; }
        public int Status { get; set; }

        public virtual ICollection<PriceListOfBookingService> PriceListOfBookingServices { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
