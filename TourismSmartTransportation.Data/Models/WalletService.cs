using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class WalletService
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public Guid ServiceId { get; set; }
        public int Status { get; set; }

        public virtual Service Service { get; set; }
        public virtual Wallet Wallet { get; set; }
    }
}
