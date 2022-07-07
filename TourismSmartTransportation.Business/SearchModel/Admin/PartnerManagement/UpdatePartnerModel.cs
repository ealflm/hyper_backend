using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PartnerManagement
{
    public class UpdatePartnerModel : FileViewModel
    {
        // public virtual string FirstName { get; set; }
        // public virtual string LastName { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string Password { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        [AllowNullOrEmptyAndCheckValidPhone]
        public virtual string Phone { get; set; }
        [AllowEmptyAndChekcValidEmail]
        public virtual string Email { get; set; }
        public virtual DateTime? DateOfBirth { get; set; }
        public virtual bool Gender { get; set; }
        public virtual int? Status { get; set; }
    }
}