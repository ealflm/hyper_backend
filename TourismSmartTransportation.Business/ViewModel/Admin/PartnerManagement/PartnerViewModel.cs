using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Admin.PartnerManagement
{
    public class PartnerViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoUrl { get; set; }
        public bool Gender { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Status { get; set; }
    }
}