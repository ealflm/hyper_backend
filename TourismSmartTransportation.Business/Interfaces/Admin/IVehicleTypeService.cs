using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Vehicle;
using TourismSmartTransportation.Business.ViewModel.Admin.Vehicle;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IVehicleTypeService
    {
        Task<List<VehicleTypeViewModel>> GetListVehicleTypes();
        Task<VehicleTypeViewModel> GetVehicleType(Guid id);
        Task<VehicleTypeViewModel> CreateVehicleType(VehicleTypeSearchModel model);
        Task<bool> UpdateVehicleType(Guid id, VehicleTypeSearchModel model);
        Task<bool> DeleteVehicleType(Guid id);
    }
}