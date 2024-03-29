﻿using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class RentStation
    {
        public RentStation()
        {
            Vehicles = new HashSet<Vehicle>();
        }

        public Guid RentStationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public Guid PartnerId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int Status { get; set; }

        public virtual Partner Partner { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
