using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;
using TourismSmartTransportation.Data.MongoDBContext;
using TourismSmartTransportation.Data.MongoCollections.Vehicle;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.CommonModel;
using System;

namespace TourismSmartTransportation.Business.Implements.Vehicle
{
    public class VehicleCollectionService : IVehicleCollectionService
    {
        private readonly IMongoCollection<VehicleCollection> _vehicles;

        public VehicleCollectionService(MongoDBContext mongo, IMongoCosmosDBSettings settings)
        {
            _vehicles = mongo.GetVehiclesCollection;
        }

        /// <summary>
        /// Create a new record
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public async Task<Response> Create(AddVehicleCollectionModel vehicle)
        {
            VehicleCollection vehicleView = new VehicleCollection()
            {
                VehicleId = vehicle.VehicleId,
                Longitude = vehicle.Longitude,
                Latitude = vehicle.Latitude,
                CreatedDate = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                ModifiedDate = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
            };
            await _vehicles.InsertOneAsync(vehicleView);
            return new()
            {
                StatusCode = 200,
                Message = "Tạo mới thành công!"
            };
        }

        /// <summary>
        /// Delete record by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Response> Delete(string id)
        {
            if (id == null)
            {
                await _vehicles.DeleteManyAsync(Builders<VehicleCollection>.Filter.Empty);
                return new()
                {
                    StatusCode = 200,
                    Message = "Xóa thành công!"
                };
            }
            var vehicle = GetById(id);
            if (vehicle == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy"
                };
            }
            await _vehicles.DeleteOneAsync(vehicle => vehicle.Id == id);
            return new()
            {
                StatusCode = 200,
                Message = "Xóa thành công!"
            };
        }

        /// <summary>
        /// Get all record in this collection
        /// </summary>
        /// <returns></returns>
        public async Task<List<VehicleCollection>> GetAll()
        {
            return await _vehicles.Find(vehicle => true).ToListAsync();
        }

        /// <summary>
        /// Get record by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<VehicleCollection> GetById(string id)
        {
            var vehicle = await _vehicles.Find(vehicle => vehicle.Id == id).FirstOrDefaultAsync();
            if (vehicle == null)
            {
                return null;
            }
            return vehicle;
        }

        /// <summary>
        /// Get record by VehicleId
        /// </summary>
        /// <param name="vehicleId"></param>
        /// <returns></returns>
        public async Task<VehicleCollection> GetByVehicleId(Guid vehicleId)
        {
            var vehicle = await _vehicles.Find(vehicle => vehicle.VehicleId.Equals(vehicleId)).ToListAsync();
            if (vehicle.Count == 0)
            {
                return null;
            }
            return vehicle[vehicle.Count - 1];
        }

        public Task<Response> Update(VehicleCollection vehicle)
        {
            throw new System.NotImplementedException();
        }
    }
}