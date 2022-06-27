using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.PriceRentingServiceConfig;
using TourismSmartTransportation.Business.ViewModel.Admin.PriceRentingServiceViewModel;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IPriceRentingServiceConfig
    {
        Task<List<PriceRentingServiceViewModel>> Search(PriceRentingServiceSearchModel model);
        Task<PriceRentingServiceViewModel> GetById(Guid id);
        Task<Response> CreatePrice(CreatePriceRentingServiceModel model);
        Task<Response> UpdatePrice(Guid id, UpdatePriceRentingServiceModel model);
        Task<Response> DeletePrice(Guid id);
    }
}