
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Authorization
{
    public class RegisterSearchModel : FileViewModel
    {
        [NotAllowedEmptyStringValidator]
        public string Username { get; set; }
        [NotAllowedEmptyStringValidator]
        public string FirstName { get; set; }
        [NotAllowedEmptyStringValidator]
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Phone]
        public string Phone { get; set; }
        public bool Gender { get; set; }
    }
}
