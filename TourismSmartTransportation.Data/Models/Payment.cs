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
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreateTime { get; set; }
        public int Method { get; set; }
        public int Status { get; set; }

        public virtual Order Order { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
