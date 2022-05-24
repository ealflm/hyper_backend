using System;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.Tier;
using TourismSmartTransportation.Business.ViewModel.Admin.Tier;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface ITierService
    {
        Task<SearchResultViewModel<TierViewModel>> SearchTier(TierSearchModel model);
        Task<TierViewModel> GetTier(Guid id);
        Task<Response> CreateTier(CreateTierModel model);
        Task<Response> UpdateTier(Guid id, UpdateTierModel model);
        Task<Response> DeleteTier(Guid id);
    }
}