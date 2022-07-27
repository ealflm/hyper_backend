using System;
using System.Collections.Generic;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.OrderDetail;
using TourismSmartTransportation.Business.ViewModel.Admin.PurchaseHistory;
using TourismSmartTransportation.Business.ViewModel.Shared;

namespace TourismSmartTransportation.Business.ViewModel.Admin.PartnerManagement
{
    public class PartnerHistoryViewModel
    {
        public WalletViewModel Wallet { get; set; }
        public List<PartnerOrderViewModel> Orders { get; set; }
    }

    public class PartnerOrderViewModel
    {
        public Guid OrderId { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public string ServiceTypeName { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public Guid? PartnerId { get; set; }
        public List<OrderDetailsInfo> OrderDetails { get; set; }
        public List<TransactionViewModel> Transactions { get; set; }
    }
}