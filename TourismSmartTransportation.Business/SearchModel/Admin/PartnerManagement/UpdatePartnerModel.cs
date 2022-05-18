using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PartnerManagement
{
    public class UpdatePartnerModel : FileViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Password { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Phone { get; set; }
        [AllowEmptyAndChekcValidEmail]
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool Gender { get; set; }
        public int? Status { get; set; }
    }
}