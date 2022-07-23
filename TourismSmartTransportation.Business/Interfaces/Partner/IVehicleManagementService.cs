using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement;
using TourismSmartTransportation.Business.ViewModel.Partner.VehicleManagement;

namespace TourismSmartTransportation.Business.Interfaces.Partner
{
    public interface IVehicleManagementService
    {
        Task<List<VehicleViewModel>> Search(VehicleSearchModel model);
        Task<VehicleViewModel> GetById(Guid id);
        Task<Response> Create(CreateVehicleModel model);
        Task<Response> Update(Guid id, UpdateVehicleModel model);
        Task<Response> Delete(Guid id);
    }
}