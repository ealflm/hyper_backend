using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Partner.DriverManagement;
using TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Partner.DriverManagement;
using TourismSmartTransportation.Business.ViewModel.Partner.VehicleManagement;

namespace TourismSmartTransportation.Business.Interfaces.Partner
{
    public interface IDriverManagementService
    {
        Task<List<DriverViewModel>> Search(DriverSearchModel model);
        Task<DriverViewModel> GetById(Guid id);
        Task<Response> Create(CreateDriverModel model);
        Task<Response> Update(Guid id, UpdateDriverModel model, bool isAssignDriverForTrip = false, bool isSaveAsync = true);
        Task<Response> Delete(Guid id);
        Task<List<VehicleViewModel>> GetVehicleListDropdownOptions(VehicleDropdownOptionsModel model);
        Task<SearchResultViewModel<DriverHistoryViewModel>> GetDriverHistory(DriverTripHistorySearchModel model);
    }
}