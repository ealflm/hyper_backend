using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class OrderDetailOfRentingService
    {
        public Guid OrderDetailId { get; set; }
        public Guid OrderId { get; set; }
        public Guid? PriceOfRentingServiceId { get; set; }
        public decimal Price { get; set; }
        public string LicensePlates { get; set; }
        public string Content { get; set; }
        public int Quantity { get; set; }
        public int ModePrice { get; set; }
        public int Status { get; set; }

        public virtual Order Order { get; set; }
        public virtual PriceOfRentingService PriceOfRentingService { get; set; }
    }
}
