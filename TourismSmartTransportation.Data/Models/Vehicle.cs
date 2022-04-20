using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Vehicle
    {
        public Vehicle()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public Guid Id { get; set; }
        public string Producer { get; set; }
        public int YearOfManufacture { get; set; }
        public string LicensePlates { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid? RentStationId { get; set; }
        public Guid? DriverId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid VehicleTypeId { get; set; }
        public int Status { get; set; }

        public virtual Company Company { get; set; }
        public virtual Driver Driver { get; set; }
        public virtual VehicleType VehicleType { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
