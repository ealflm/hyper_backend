using System;
using System.Collections.Generic;

#nullable disable

namespace TourismSmartTransportation.Data.Models
{
    public partial class Admin
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoUrl { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public int Status { get; set; }
    }
}
