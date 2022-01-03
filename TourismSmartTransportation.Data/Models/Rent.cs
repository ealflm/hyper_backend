using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Rent
    {
        public Rent()
        {
            Transactions = new HashSet<Transaction>();
        }

        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public decimal Price { get; set; }
        public Guid CustomerId { get; set; }
        public int QuantityPeople { get; set; }
        public DateTime Time { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid? TripId { get; set; }
        public decimal StartLatitude { get; set; }
        public decimal StartLongitude { get; set; }
        public decimal EndLatitude { get; set; }
        public decimal EndLongitude { get; set; }
        public int Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ServiceType ServiceType { get; set; }
        public virtual Trip Trip { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
