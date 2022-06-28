using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.CategoryManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.CategoryManagement;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface ICategoryManagementService
    {
        Task<SearchResultViewModel<CategoryViewModel>> GetAll(CategorySearchModel model);
        Task<CategoryViewModel> Get(Guid id);
        Task<Response> Add(CreateCategoryModel model);
        Task<Response> Update(Guid id, UpdateCategoryModel model);
        Task<Response> Delete(Guid id);
    }
}
