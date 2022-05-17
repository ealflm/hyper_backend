using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Transaction
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public Guid WalletId { get; set; }
        public decimal Amount { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }

        public virtual Payment Payment { get; set; }
        public virtual Wallet Wallet { get; set; }
    }
}
