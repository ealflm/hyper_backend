using System.ComponentModel.DataAnnotations;


namespace TourismSmartTransportation.Business.SearchModel.Admin.Authorization
{
    public class LoginSearchModel
    {
        [Required, EmailAddress] public string EmailAddress { get; set; }

        [Required] public string Password { get; set; }
    }
}
