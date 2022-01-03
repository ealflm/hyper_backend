using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Vehicle
    {
        public Vehicle()
        {
            Rents = new HashSet<Rent>();
        }

        public Guid Id { get; set; }
        public string Producer { get; set; }
        public int YearOfManufacture { get; set; }
        public string LicensePlates { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longtitude { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid? RentStationId { get; set; }
        public Guid? DriverId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? TripId { get; set; }
        public Guid VehicleTypeId { get; set; }
        public int Status { get; set; }

        public virtual Company Company { get; set; }
        public virtual Driver Driver { get; set; }
        public virtual RentStation RentStation { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual VehicleType VehicleType { get; set; }
        public virtual ICollection<Rent> Rents { get; set; }
    }
}
