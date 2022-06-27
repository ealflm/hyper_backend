using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.VehicleType;
using TourismSmartTransportation.Business.ViewModel.Admin.VehicleType;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IVehicleTypeService
    {
        Task<List<VehicleTypeViewModel>> GetListVehicleTypes(VehicleTypeSearchModel model);
        Task<VehicleTypeViewModel> GetVehicleType(Guid id);
        Task<Response> CreateVehicleType(CreateVehicleTypeModel model);
        Task<Response> UpdateVehicleType(Guid id, CreateVehicleTypeModel model);
        Task<Response> DeleteVehicleType(Guid id);
    }
}