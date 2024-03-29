﻿using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.PurchaseHistory
{
    public class TransactionViewModel
    {
        public Guid OrderId { get; set; }
        public Guid WalletId { get; set; }
        public Guid? PartnerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
    }
}
