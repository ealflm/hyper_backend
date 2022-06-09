namespace TourismSmartTransportation.Data.MongoDBContext
{
    public class MongoCosmosDBSettings : IMongoCosmosDBSettings
    {
        public string DatabaseName { get; set; }
        public string MongoConnectionString { get; set; }
    }

    public interface IMongoCosmosDBSettings
    {
        string DatabaseName { get; set; }
        string MongoConnectionString { get; set; }
    }
}
