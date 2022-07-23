using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Partner.DriverManagement
{
    public class DriverForTripModel
    {
        [Required]
        public Guid PartnerId { get; set; }
    }
}