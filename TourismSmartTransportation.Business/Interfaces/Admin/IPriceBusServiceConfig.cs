using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.PriceBusServiceConfig;
using TourismSmartTransportation.Business.ViewModel.Admin.PriceBusServiceViewModel;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IPriceBusServiceConfig
    {
        Task<List<PriceBusServiceViewModel>> Search(PriceBusServiceSearchModel model);
        Task<PriceBusServiceViewModel> GetById(Guid id);
        Task<Response> CreatePrice(CreatePriceBusServiceModel model);
        Task<Response> UpdatePrice(Guid id, UpdatePriceBusServiceModel model);
        Task<Response> DeletePrice(Guid id);
    }
}