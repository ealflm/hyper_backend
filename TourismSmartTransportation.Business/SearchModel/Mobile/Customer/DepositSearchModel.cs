using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.SearchModel.Mobile.Customer
{
    public class DepositSearchModel
    {
        public Guid CustomerId { get; set; }
        public decimal Amount { get; set; }
        public int Method { get; set; }
    }
}
