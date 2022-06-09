using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;
using TourismSmartTransportation.Data.MongoDBContext;
using TourismSmartTransportation.Data.MongoCollections;
using TourismSmartTransportation.Business.Interfaces;
using TourismSmartTransportation.Business.CommonModel;

namespace TourismSmartTransportation.Business.Implements
{
    public class DriverCollectionService : IDriverCollectionService
    {
        private readonly IMongoCollection<DriverCollection> _drivers;

        public DriverCollectionService(MongoDBContext mongo, IMongoCosmosDBSettings settings)
        {
            _drivers = mongo.GetDriversCollection;
        }

        public async Task<Response> Create(DriverCollection driver)
        {
            await _drivers.InsertOneAsync(driver);
            return new()
            {
                StatusCode = 200,
                Message = "Tạo mới thành công!"
            };
        }

        public async Task<Response> Delete(string id)
        {
            await _drivers.DeleteOneAsync(driver => driver.Id == id);
            return new()
            {
                StatusCode = 200,
                Message = "Xóa thành công"
            };
        }

        public async Task<List<DriverCollection>> GetAll()
        {
            return await _drivers.Find(driver => true).ToListAsync();
        }

        public async Task<DriverCollection> GetById(string id)
        {
            var driver = await _drivers.Find(driver => driver.Id == id).FirstOrDefaultAsync();
            if (driver == null)
            {
                return null;
            }
            return driver;
        }

        public Task<Response> Update(DriverCollection driver)
        {
            throw new System.NotImplementedException();
        }
    }
}