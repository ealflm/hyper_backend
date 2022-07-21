using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Shared
{
    public class PasswordVerificationModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string OldPassowrd { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}