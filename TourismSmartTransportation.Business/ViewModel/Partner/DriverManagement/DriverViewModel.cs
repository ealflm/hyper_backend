using System;

namespace TourismSmartTransportation.Business.ViewModel.Partner.DriverManagement
{
    public class DriverViewModel
    {
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        public Guid? VehicleId { get; set; }
        public string VehicleName { get; set; }
        public string VehicleTypeLabel { get; set; }
        public string LicensePlates { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoUrl { get; set; }
        public bool Gender { get; set; }
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Status { get; set; }
    }
}