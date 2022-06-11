using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Wallet
    {
        public Wallet()
        {
            Payments = new HashSet<Payment>();
        }

        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public decimal AccountBalance { get; set; }
        public int Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
