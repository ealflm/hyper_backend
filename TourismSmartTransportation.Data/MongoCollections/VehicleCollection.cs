using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TourismSmartTransportation.Data.MongoCollections
{
    public class VehicleCollection
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Id { get; set; }
        public string VehicleId { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
    }
}