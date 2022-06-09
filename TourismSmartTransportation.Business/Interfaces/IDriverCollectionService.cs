using System.Threading.Tasks;
using System.Collections.Generic;
using TourismSmartTransportation.Data.MongoCollections;
using TourismSmartTransportation.Business.CommonModel;

namespace TourismSmartTransportation.Business.Interfaces
{
    public interface IDriverCollectionService
    {
        public Task<List<DriverCollection>> GetAll();
        public Task<DriverCollection> GetById(string id);
        public Task<Response> Create(DriverCollection product);
        public Task<Response> Update(DriverCollection product);
        public Task<Response> Delete(string id);
    }
}
