using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.PriceBookingServiceConfig;
using TourismSmartTransportation.Business.ViewModel.Admin.PriceBookingServiceViewModel;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IPriceBookingServiceConfig
    {
        Task<List<PriceBookingServiceViewModel>> Search(PriceBookingServiceSearchModel model);
        Task<PriceBookingServiceViewModel> GetById(Guid id);
        Task<Response> CreatePrice(CreatePriceBookingServiceModel model);
        Task<Response> UpdatePrice(Guid id, UpdatePriceBookingServiceModel model);
        Task<Response> DeletePrice(Guid id);
    }
}