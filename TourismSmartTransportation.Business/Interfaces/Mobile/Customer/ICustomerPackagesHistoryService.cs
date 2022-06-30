using System;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;

namespace TourismSmartTransportation.Business.Interfaces.Mobile.Customer
{
    public interface ICustomerPackagesHistoryService
    {
        Task<SearchResultViewModel<CustomerPackagesHistoryViewModel>> GetCustomerPackageHistory(CustomerPackagesHistorySearchModel model);
    }
}