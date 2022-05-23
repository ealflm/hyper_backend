using System;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Shared;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Shared;

namespace TourismSmartTransportation.Business.Interfaces.Shared
{
    public interface ICustomerTierHistoryService
    {
        Task<SearchResultViewModel<CustomerTierHistoryViewModel>> Get(CustomerTierHistorySearchModel model);
    }
}