using System;
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

        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Status { get; set; }

        public virtual Partner Partner { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
