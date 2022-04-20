using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class WalletType
    {
        public WalletType()
        {
            Wallets = new HashSet<Wallet>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
