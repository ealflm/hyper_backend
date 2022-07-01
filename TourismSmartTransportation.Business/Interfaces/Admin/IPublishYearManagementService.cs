using System;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.PublishYearManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.PublishYearManagement;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IPublishYearManagementService
    {
        Task<SearchResultViewModel<PublishYearViewModel>> GetAll(PublishYearSearchModel model);
        Task<PublishYearViewModel> Get(Guid id);
        Task<Response> Add(CreatePublishYearModel model);
        Task<Response> Update(Guid id, UpdatePublishYearModel model);
        Task<Response> Delete(Guid id);
    }
}
