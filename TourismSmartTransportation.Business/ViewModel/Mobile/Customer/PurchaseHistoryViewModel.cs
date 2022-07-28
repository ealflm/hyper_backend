using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.ViewModel.Admin.PurchaseHistory;

namespace TourismSmartTransportation.Business.ViewModel.Mobile.Customer
{
    public class PurchaseHistoryViewModel
    {
        public List<OrderViewModel> Orders { get; set; }
        public List<TransactionViewModel> Transactions { get; set; }
        public List<CustomerTripViewModel> CustomerTrips { get; set; }
    }
}
