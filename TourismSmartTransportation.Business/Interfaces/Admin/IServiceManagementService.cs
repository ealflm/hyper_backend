using System;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.Service;
using TourismSmartTransportation.Business.ViewModel.Admin.Service;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IServiceManagementService
    {
        Task<SearchServiceResultViewModel> SearchServices(ServiceSearchModel model);
        Task<Response> GetService(Guid id);
        Task<Response> CreateService(CreateServiceModel model);
        Task<Response> UpdateService(Guid id, CreateServiceModel model);
        Task<Response> DeleteService(Guid id);
    }
}