using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.CardManagement
{
    public class CardViewModel
    {
        public Guid Id { get; set; }
        public Guid? CustomerId { get; set; }
        public string Uid { get; set; }
        public int Status { get; set; }
    }
}