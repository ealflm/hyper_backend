using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.Vehicle;
using TourismSmartTransportation.Business.ViewModel.Admin.Vehicle;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IVehicleTypeService
    {
        Task<List<VehicleTypeViewModel>> GetListVehicleTypes();
        Task<Response> GetVehicleType(Guid id);
        Task<Response> CreateVehicleType(CreateVehicleModel model);
        Task<Response> UpdateVehicleType(Guid id, CreateVehicleModel model);
        Task<Response> DeleteVehicleType(Guid id);
    }
}