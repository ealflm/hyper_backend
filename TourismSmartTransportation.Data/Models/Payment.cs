using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Payment
    {
        public Payment()
        {
            Transactions = new HashSet<Transaction>();
        }

        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethod { get; set; }
        public DateTime Time { get; set; }
        public Guid CustomerId { get; set; }
        public int Status { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
