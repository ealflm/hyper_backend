using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Payments = new HashSet<Payment>();
            Rents = new HashSet<Rent>();
            Wallets = new HashSet<Wallet>();
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Gender { get; set; }
        public DateTime Birthday { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public string PhotoUrl { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Rent> Rents { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
