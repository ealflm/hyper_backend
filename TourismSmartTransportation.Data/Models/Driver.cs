﻿using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Driver
    {
        public Driver()
        {
            FeedbackForDrivers = new HashSet<FeedbackForDriver>();
            Trips = new HashSet<Trip>();
        }

        public Guid DriverId { get; set; }
        public Guid PartnerId { get; set; }
        public Guid? VehicleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoUrl { get; set; }
        public bool Gender { get; set; }
        public string Phone { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Status { get; set; }
        public string RegistrationToken { get; set; }

        public virtual Partner Partner { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual ICollection<FeedbackForDriver> FeedbackForDrivers { get; set; }
        public virtual ICollection<Trip> Trips { get; set; }
    }
}
