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
        Task<bool> CreateVehicleType(CreateVehicleTypeModel model);
        Task<bool> UpdateVehicleType(Guid id, CreateVehicleTypeModel model);
        Task<bool> DeleteVehicleType(Guid id);
    }
}