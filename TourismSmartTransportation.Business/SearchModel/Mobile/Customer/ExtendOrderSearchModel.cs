using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.SearchModel.Mobile.Customer
{
    public class ExtendOrderSearchModel
    {
        public Guid CustomerTripId { get; set; }
        public Guid? PriceOfRentingServiceId { get; set; }
        public decimal Price { get; set; }
        public string Content { get; set; }
        public int Quantity { get; set; }
        public int ModePrice { get; set; }
        public bool MergeOrder { get; set; }
    }
}
