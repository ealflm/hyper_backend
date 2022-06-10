using System.Threading.Tasks;
using System.Collections.Generic;
using TourismSmartTransportation.Data.MongoCollections;
using TourismSmartTransportation.Business.CommonModel;

namespace TourismSmartTransportation.Business.Interfaces
{
    public interface IVehicleCollectionService
    {
        public Task<List<VehicleCollection>> GetAll();
        public Task<VehicleCollection> GetById(string id);
        public Task<Response> Create(VehicleCollection product);
        public Task<Response> Update(VehicleCollection product);
        public Task<Response> Delete(string id);
    }
}
