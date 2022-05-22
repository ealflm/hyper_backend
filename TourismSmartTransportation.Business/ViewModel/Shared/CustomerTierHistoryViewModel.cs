using System;

namespace TourismSmartTransportation.Business.ViewModel.Shared
{
    public class CustomerTierHistoryViewModel
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid TierId { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public int Status { get; set; }
    }
}