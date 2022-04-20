using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.Service
{
    public class ServiceViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string PhotoUrls { get; set; }
        public int Status { get; set; }
    }
}