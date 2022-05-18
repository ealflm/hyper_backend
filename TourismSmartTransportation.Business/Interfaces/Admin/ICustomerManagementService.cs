using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Admin.CustomerManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.CustomerManagement;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface ICustomerManagementService
    {
        Task<SearchResultViewModel<CustomerViewModel>> SearchCustomer(CustomerSearchModel model);
        Task<CustomerViewModel> GetCustomer(Guid id);
        Task<bool> AddCustomer(AddCustomerModel model);
        Task<bool> UpdateCustomer(Guid id, UpdateCustomerModel model);
        Task<bool> DeleteCustomer(Guid id);
    }
}
