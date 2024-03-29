using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TourismSmartTransportation.Data.MongoCollections.Vehicle
{
    public class VehicleCollectionViewModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Id { get; set; }
        public Guid VehicleId { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public Int32 CreatedDate { get; set; }
        public Int32 ModifiedDate { get; set; }
    }
}