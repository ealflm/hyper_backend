using MongoDB.Driver;
using TourismSmartTransportation.Data.MongoCollections.Notification;
using TourismSmartTransportation.Data.MongoCollections.Vehicle;

namespace TourismSmartTransportation.Data.MongoDBContext
{
    public class MongoDBContext
    {
        private string _connectionStrings = string.Empty;
        private string _databaseName = string.Empty;
        private string _collectionName = string.Empty;


        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;

        public MongoDBContext(IMongoCosmosDBSettings settings)
        {
            this._connectionStrings = settings.MongoConnectionString;
            this._databaseName = settings.DatabaseName;
            this._client = new MongoClient(_connectionStrings);
            this._database = _client.GetDatabase(_databaseName);
        }

        public IMongoClient GetClient
        {
            get { return _client; }
        }

        public IMongoDatabase Database
        {
            get { return _database; }
        }

        public IMongoCollection<VehicleCollection> GetVehiclesCollection
        {
            get { return _database.GetCollection<VehicleCollection>("Vehicles"); }
        }

        public IMongoCollection<NotificationCollection> GetNotificationsCollection
        {
            get { return _database.GetCollection<NotificationCollection>("Notifications"); }
        }
    }
}