using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Partner.DriverManagement;
using TourismSmartTransportation.Business.ViewModel.Partner.DriverManagement;

namespace TourismSmartTransportation.Business.Interfaces.Partner
{
    public interface IDriverManagementService
    {
        Task<List<DriverViewModel>> Search(DriverSearchModel model);
        Task<DriverViewModel> GetById(Guid id);
        Task<Response> Create(CreateDriverModel model);
        Task<Response> Update(Guid id, UpdateDriverModel model);
        Task<Response> Delete(Guid id);
    }
}