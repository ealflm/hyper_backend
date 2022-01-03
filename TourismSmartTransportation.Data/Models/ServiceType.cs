using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class ServiceType
    {
        public ServiceType()
        {
            Rents = new HashSet<Rent>();
            ServiceDetails = new HashSet<ServiceDetail>();
            Vehicles = new HashSet<Vehicle>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Code { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Rent> Rents { get; set; }
        public virtual ICollection<ServiceDetail> ServiceDetails { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
