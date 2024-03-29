﻿using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class PriceOfRentingService
    {
        public PriceOfRentingService()
        {
            OrderDetailOfRentingServices = new HashSet<OrderDetailOfRentingService>();
            Vehicles = new HashSet<Vehicle>();
        }

        public Guid PriceOfRentingServiceId { get; set; }
        public Guid PublishYearId { get; set; }
        public Guid CategoryId { get; set; }
        public decimal MinTime { get; set; }
        public decimal MaxTime { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal FixedPrice { get; set; }
        public decimal WeekendPrice { get; set; }
        public decimal HolidayPrice { get; set; }
        public int Status { get; set; }

        public virtual Category Category { get; set; }
        public virtual PublishYear PublishYear { get; set; }
        public virtual ICollection<OrderDetailOfRentingService> OrderDetailOfRentingServices { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
