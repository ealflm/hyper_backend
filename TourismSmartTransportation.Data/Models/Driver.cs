using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Driver
    {
        public Driver()
        {
            Vehicles = new HashSet<Vehicle>();
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Gender { get; set; }
        public int YearOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoUrl { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public Guid CompanyId { get; set; }
        public int Status { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
