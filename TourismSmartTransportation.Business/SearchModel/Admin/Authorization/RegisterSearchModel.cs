
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validate;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Authorization
{
    public class RegisterSearchModel
    {
        [NotAllowedEmptyStringValidator]
        public string FirstName { get; set; }
        [NotAllowedEmptyStringValidator]
        public string LastName { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
