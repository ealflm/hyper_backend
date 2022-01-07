using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Admin.CompanyManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.CompanyManagement;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface ICompanyManagementService
    {
        Task<SearchResultViewModel> SearchCompany(CompanySearchModel model);
        Task<CompanyViewModel> GetCompany(Guid id);
        Task<bool> AddCompany(AddCompanyViewModel model);
        Task<bool> UpdateCompany(Guid id, AddCompanyViewModel model);
        Task<bool> DeleteCompany(Guid id);
    }
}
