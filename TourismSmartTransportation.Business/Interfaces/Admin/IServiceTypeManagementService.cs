using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Admin.ServiceType;
using TourismSmartTransportation.Business.ViewModel.Admin.ServiceTypeManagement;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IServiceTypeManagementService
    {
        Task<ServiceTypeViewModel> Get(Guid id);
        Task<bool> Create(ServiceTypeSearchModel model);
        Task<bool> Update(Guid id, ServiceTypeSearchModel model);
        Task<bool> Delete(Guid id);
        Task<SearchResultViewModel<ServiceTypeViewModel>> GetAll();
        Task<List<ServiceTypeViewModel>> GetListByPartnerId(Guid id);
    }
}
