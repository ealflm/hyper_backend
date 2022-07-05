using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Partner.DriverManagement
{
    public class CreateDriverModel
    {
        [Required]
        public Guid PartnerId { get; set; }
        public Guid? VehicleId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public bool Gender { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; }
    }
}