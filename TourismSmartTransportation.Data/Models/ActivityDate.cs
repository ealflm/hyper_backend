using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class ActivityDate
    {
        public ActivityDate()
        {
            Vehicles = new HashSet<Vehicle>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
