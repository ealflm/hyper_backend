using System;
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
        // [StringLength(255)]
        // [Required]
        // public string Username { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string CompanyName { get; set; }
        // [Required]
        // public string Password { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
        [AllowEmptyAndChekcValidEmail]
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// true is male,
        /// false is female
        /// </summary>
        /// <value></value>
        public bool Gender { get; set; }
        [Required]
        public List<Guid> ServiceTypeIdList { get; set; }
    }
}