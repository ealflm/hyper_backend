using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.PublishYearManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.PublishYearManagement;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IPublishYearManagementService
    {
        Task<SearchResultViewModel<PublishYearViewModel>> GetAll();
        Task<PublishYearViewModel> Get(Guid id);
        Task<Response> Add(PublishYearSearchModel model);
        Task<Response> Update(Guid id, PublishYearSearchModel model);
        Task<Response> Delete(Guid id);
    }
}
