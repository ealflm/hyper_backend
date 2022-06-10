using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;
using TourismSmartTransportation.Data.MongoDBContext;
using TourismSmartTransportation.Data.MongoCollections;
using TourismSmartTransportation.Business.Interfaces;
using TourismSmartTransportation.Business.CommonModel;

namespace TourismSmartTransportation.Business.Implements
{
    public class VehicleCollectionService : IVehicleCollectionService
    {
        private readonly IMongoCollection<VehicleCollection> _vehicles;

        public VehicleCollectionService(MongoDBContext mongo, IMongoCosmosDBSettings settings)
        {
            _vehicles = mongo.GetVehiclesCollection;
        }

        public async Task<Response> Create(VehicleCollection vehicle)
        {
            await _vehicles.InsertOneAsync(vehicle);
            return new()
            {
                StatusCode = 200,
                Message = "Tạo mới thành công!"
            };
        }

        public async Task<Response> Delete(string id)
        {
            await _vehicles.DeleteOneAsync(vehicle => vehicle.Id == id);
            return new()
            {
                StatusCode = 200,
                Message = "Xóa thành công!"
            };
        }

        public async Task<List<VehicleCollection>> GetAll()
        {
            return await _vehicles.Find(vehicle => true).ToListAsync();
        }

        public async Task<VehicleCollection> GetById(string id)
        {
            var vehicle = await _vehicles.Find(vehicle => vehicle.Id == id).FirstOrDefaultAsync();
            if (vehicle == null)
            {
                return null;
            }
            return vehicle;
        }

        public Task<Response> Update(VehicleCollection vehicle)
        {
            throw new System.NotImplementedException();
        }
    }
}