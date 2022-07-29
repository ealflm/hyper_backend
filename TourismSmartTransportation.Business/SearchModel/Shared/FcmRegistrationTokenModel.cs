using System;
using System.ComponentModel.DataAnnotations;

namespace TourismSmartTransportation.Business.SearchModel.Shared
{
    public class FcmRegistrationTokenModel
    {
        // Id can include: CustomerId, PartnerId,... Some entity want to connect with Firebase cloud messaging
        [Required]
        public Guid EntityId { get; set; }

        [Required]
        public string RegistrationToken { get; set; }
    }
}