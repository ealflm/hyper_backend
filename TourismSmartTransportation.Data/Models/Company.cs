using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Company
    {
        public Company()
        {
            CompanyTrips = new HashSet<CompanyTrip>();
            Drivers = new HashSet<Driver>();
            Vehicles = new HashSet<Vehicle>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public string Address { get; set; }
        public string PhotoUrl { get; set; }
        public int Status { get; set; }

        public virtual ICollection<CompanyTrip> CompanyTrips { get; set; }
        public virtual ICollection<Driver> Drivers { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
