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

        public Guid Id { get; set; }
        public decimal AccountBalance { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public Guid CustomerId { get; set; }
        public Guid WalletTypeId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual WalletType WalletType { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
