﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Validation;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PartnerManagement
{
    public class AddPartnerModel : FileViewModel
    {
        [NotAllowedEmptyStringValidator]
        [StringLength(255)]
        public string Username { get; set; }
        [NotAllowedEmptyStringValidator]
        public string FirstName { get; set; }
        [NotAllowedEmptyStringValidator]
        public string LastName { get; set; }
        [NotAllowedEmptyStringValidator]
        public string CompanyName { get; set; }
        [NotAllowedEmptyStringValidator]
        public string Password { get; set; }
        [NotAllowedEmptyStringValidator]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Phone { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        [Range(0, 1)]
        public bool Gender { get; set; }
        public int? Status { get; set; }
    }
}