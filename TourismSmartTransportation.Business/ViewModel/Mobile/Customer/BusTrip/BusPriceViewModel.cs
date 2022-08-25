using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Mobile.Customer.BusTrip
{
    public class BusPriceViewModel
    {
        public string Name { get; set; }
        public int TotalStation { get; set; }
        public decimal Distance { get; set; }
        public decimal Price { get; set; }
        public bool IsUsePackage { get; set; }
    }
}
