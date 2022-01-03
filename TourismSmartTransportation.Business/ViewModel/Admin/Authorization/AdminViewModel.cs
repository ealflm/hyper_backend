using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.Authorization
{
    public class AdminViewModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoUrl { get; set; }
        public int Status { get; set; }
    }
}
