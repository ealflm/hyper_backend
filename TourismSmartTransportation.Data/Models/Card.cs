using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Card
    {
        public Guid Id { get; set; }
        public string Uid { get; set; }
        public Guid WalletId { get; set; }

        public virtual Wallet Wallet { get; set; }
    }
}
