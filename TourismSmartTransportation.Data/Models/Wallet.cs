using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Wallet
    {
        public Wallet()
        {
            Cards = new HashSet<Card>();
            Transactions = new HashSet<Transaction>();
            WalletServices = new HashSet<WalletService>();
        }

        public Guid Id { get; set; }
        public decimal AccountBalance { get; set; }
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<WalletService> WalletServices { get; set; }
    }
}
