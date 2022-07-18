using System;

namespace TourismSmartTransportation.Business.SearchModel.Mobile.Customer
{
    public class WalletSearchModel
    {
        public Guid? WalletId { get; set; }
        public Guid? CustomerId { get; set; }
        public int Status { get; set; }
    }
}