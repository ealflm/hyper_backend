﻿using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class ServiceType
    {
        public ServiceType()
        {
            PackageStatuses = new HashSet<PackageStatus>();
            Packages = new HashSet<Package>();
            PartnerServiceTypes = new HashSet<PartnerServiceType>();
            PriceBookings = new HashSet<PriceBooking>();
            Vehicles = new HashSet<Vehicle>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Status { get; set; }

        public virtual ICollection<PackageStatus> PackageStatuses { get; set; }
        public virtual ICollection<Package> Packages { get; set; }
        public virtual ICollection<PartnerServiceType> PartnerServiceTypes { get; set; }
        public virtual ICollection<PriceBooking> PriceBookings { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
