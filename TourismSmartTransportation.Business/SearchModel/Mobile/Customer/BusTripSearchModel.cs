using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.SearchModel.Mobile.Customer
{
    public class BusTripSearchModel
    {
        public decimal StartLongitude { get; set; }
        public decimal StartLatitude { get; set; }
        public decimal EndLongitude { get; set; }
        public decimal EndLatitude { get; set; }
    }
}
