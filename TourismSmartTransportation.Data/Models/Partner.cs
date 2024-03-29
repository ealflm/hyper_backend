﻿using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Partner
    {
        public Partner()
        {
            Drivers = new HashSet<Driver>();
            Orders = new HashSet<Order>();
            PartnerRoutes = new HashSet<PartnerRoute>();
            PartnerServiceTypes = new HashSet<PartnerServiceType>();
            RentStations = new HashSet<RentStation>();
            Vehicles = new HashSet<Vehicle>();
            Wallets = new HashSet<Wallet>();
        }

        public Guid PartnerId { get; set; }
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoUrl { get; set; }
        public bool Gender { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int Status { get; set; }
        public string CompanyName { get; set; }
        public string RegistrationToken { get; set; }

        public virtual ICollection<Driver> Drivers { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<PartnerRoute> PartnerRoutes { get; set; }
        public virtual ICollection<PartnerServiceType> PartnerServiceTypes { get; set; }
        public virtual ICollection<RentStation> RentStations { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
