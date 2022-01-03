using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Transaction
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public DateTime TransactionTime { get; set; }
        public decimal Amount { get; set; }
        public Guid RentId { get; set; }
        public Guid PaymentId { get; set; }
        public int Status { get; set; }

        public virtual Payment Payment { get; set; }
        public virtual Rent Rent { get; set; }
        public virtual Wallet Wallet { get; set; }
    }
}
