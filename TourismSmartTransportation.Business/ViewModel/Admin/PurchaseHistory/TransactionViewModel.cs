using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Admin.PurchaseHistory
{
    public class TransactionViewModel
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public Guid WalletId { get; set; }
        public decimal Amount { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
    }
}
