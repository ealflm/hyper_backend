using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;
using TourismSmartTransportation.Data.MongoDBContext;
using TourismSmartTransportation.Data.MongoCollections.Vehicle;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.CommonModel;
using System;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement;
using TourismSmartTransportation.Business.ViewModel.Partner.VehicleManagement;

namespace TourismSmartTransportation.Business.Implements.Vehicle
{
    public class VehicleCollectionService : IVehicleCollectionService
    {
        private readonly IMongoCollection<VehicleCollection> _vehicles;
        private readonly IVehicleManagementService _vehicleManagementService;

        public VehicleCollectionService(MongoDBContext mongo, IMongoCosmosDBSettings settings, IVehicleManagementService vehicleManagementService)
        {
            _vehicles = mongo.GetVehiclesCollection;
            _vehicleManagementService = vehicleManagementService;
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
            UpdateVehicleModel model = new UpdateVehicleModel() { IsRunning = 1 };
            await _vehicleManagementService.Update(vehicleView.VehicleId, model); // Temp basic solution to update. 
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
            List<VehicleCollection> vehiclesTrackingList = new List<VehicleCollection>();
            VehicleSearchModel model = new VehicleSearchModel();
            var vehiclesList = await _vehicleManagementService.Search(model);
            foreach (var p in vehiclesList)
            {
                var lastedVehicleDataRealtime = await _vehicles.Find(vehicle => vehicle.VehicleId.Equals(p.Id)).ToListAsync();
                if (lastedVehicleDataRealtime.Count > 0)
                {
                    vehiclesTrackingList.Add(lastedVehicleDataRealtime[lastedVehicleDataRealtime.Count - 1]);
                }
            }
            return vehiclesTrackingList;
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
                return new VehicleCollection()
                {
                    Id = "-1"
                };
            }
            return vehicle[vehicle.Count - 1];
        }

        public Task<Response> Update(VehicleCollection vehicle)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<VehicleCollection>> GetVehiclesListByPartner(Guid partnetId)
        {
            List<VehicleCollection> vehiclesTrackingList = new List<VehicleCollection>();
            VehicleSearchModel model = new VehicleSearchModel();
            model.PartnerId = partnetId;
            var vehiclesList = await _vehicleManagementService.Search(model);
            foreach (var p in vehiclesList)
            {
                var lastedVehicleDataRealtime = await _vehicles.Find(vehicle => vehicle.VehicleId.Equals(p.Id)).ToListAsync();
                if (lastedVehicleDataRealtime.Count > 0)
                {
                    vehiclesTrackingList.Add(lastedVehicleDataRealtime[lastedVehicleDataRealtime.Count - 1]);
                }
            }
            return vehiclesTrackingList;
        }
    }
}