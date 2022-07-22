using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;

namespace TourismSmartTransportation.Business.Interfaces.Mobile.Customer
{
    public interface IRentService
    {
        Task<PriceRentingViewModel> GetPrice(string id);
        
    }
}
