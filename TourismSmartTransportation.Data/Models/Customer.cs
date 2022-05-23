using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Cards = new HashSet<Card>();
            CustomerTierHistories = new HashSet<CustomerTierHistory>();
            CustomerTrips = new HashSet<CustomerTrip>();
            Orders = new HashSet<Order>();
            Wallets = new HashSet<Wallet>();
        }

        public Guid Id { get; set; }
        public Guid? TierId { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Gender { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Status { get; set; }

        public virtual Tier Tier { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
        public virtual ICollection<CustomerTierHistory> CustomerTierHistories { get; set; }
        public virtual ICollection<CustomerTrip> CustomerTrips { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
