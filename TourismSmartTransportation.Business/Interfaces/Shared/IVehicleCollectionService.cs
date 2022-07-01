using System.Threading.Tasks;
using System.Collections.Generic;
using TourismSmartTransportation.Data.MongoCollections.Vehicle;
using TourismSmartTransportation.Business.CommonModel;
using System;

namespace TourismSmartTransportation.Business.Interfaces.Shared
{
    public interface IVehicleCollectionService
    {
        public Task<List<VehicleCollection>> GetAll();
        public Task<VehicleCollection> GetById(string id);
        public Task<VehicleCollection> GetByVehicleId(Guid vehicleId);
        public Task<Response> Create(AddVehicleCollectionModel product);
        public Task<Response> Update(VehicleCollection product);
        public Task<Response> Delete(string id);
        public Task<List<VehicleCollection>> GetVehiclesListByPartner(Guid partnerId);
    }
}
