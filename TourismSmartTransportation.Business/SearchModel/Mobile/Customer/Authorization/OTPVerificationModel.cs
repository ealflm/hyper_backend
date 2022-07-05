
using System.ComponentModel.DataAnnotations;

namespace TourismSmartTransportation.Business.SearchModel.Mobile.Customer.Authorization
{
    public class OTPVerificationModel
    {
        [Required]
        [Phone]
        public string Phone { get; set; }
        [Required]
        public string OTPCode { get; set; }
        [Required]
        public string RequestId { get; set; }
    }
}
