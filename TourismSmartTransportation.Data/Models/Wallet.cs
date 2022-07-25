using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Wallet
    {
        public Wallet()
        {
            Transactions = new HashSet<Transaction>();
        }

        public Guid WalletId { get; set; }
        public Guid? CustomerId { get; set; }
        public decimal AccountBalance { get; set; }
        public int Status { get; set; }
        public Guid? AdminId { get; set; }
        public Guid? PartnerId { get; set; }

        public virtual Admin Admin { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Partner Partner { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
