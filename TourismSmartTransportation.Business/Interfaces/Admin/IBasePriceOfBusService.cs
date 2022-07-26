using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.BasePriceOfBusService;
using TourismSmartTransportation.Business.ViewModel.Admin.BasePriceOfBusService;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IBasePriceOfBusService
    {
        Task<Response> GeneratePriceOfBusService(AddBasePriceOfBusService model);
        Task<List<BasePriceOfBusServiceViewModel>> GetBasePricesList(BasePriceOfBusServiceSearchModel model);
        Task<BasePriceOfBusServiceViewModel> GetBasePricesById(Guid id);
        Task<Response> UpdateBasePrice(Guid id, UpdateBasePriceOfBusService model);
    }
}