using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class OrderDetail
    {
        public OrderDetail()
        {
            TripActivities = new HashSet<TripActivity>();
        }

        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid VehicleId { get; set; }
        public decimal Price { get; set; }
        public int QuantityPeople { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid ServiceDetailId { get; set; }
        public int Status { get; set; }

        public virtual Order Order { get; set; }
        public virtual ServiceDetail ServiceDetail { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual ICollection<TripActivity> TripActivities { get; set; }
    }
}
