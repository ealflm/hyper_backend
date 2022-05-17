using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Vehicle
    {
        public Vehicle()
        {
            CustomerTrips = new HashSet<CustomerTrip>();
            Drivers = new HashSet<Driver>();
            Trips = new HashSet<Trip>();
        }

        public Guid Id { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid VehicleTypeId { get; set; }
        public Guid RentStationId { get; set; }
        public Guid PartnerId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid PublishYearId { get; set; }
        public Guid ActivityDateId { get; set; }
        public string Name { get; set; }
        public string LicensePlates { get; set; }
        public string Color { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int Status { get; set; }

        public virtual ActivityDate ActivityDate { get; set; }
        public virtual Category Category { get; set; }
        public virtual Partner Partner { get; set; }
        public virtual PublishYear PublishYear { get; set; }
        public virtual RentStation RentStation { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        public virtual VehicleType VehicleType { get; set; }
        public virtual ICollection<CustomerTrip> CustomerTrips { get; set; }
        public virtual ICollection<Driver> Drivers { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
    }
}
