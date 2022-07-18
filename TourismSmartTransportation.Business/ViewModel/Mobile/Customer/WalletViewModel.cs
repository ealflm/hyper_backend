using System;

namespace TourismSmartTransportation.Business.ViewModel.Mobile.Customer
{
    public class WalletViewModel
    {
        public Guid WalletId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal AccountBalance { get; set; }
        public int Status { get; set; }
    }
}