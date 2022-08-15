using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TourismSmartTransportation.Data.MongoCollections.Notification
{
    public class NotificationCollection
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public Int32 CreatedDateTimeStamp { get; set; }
        public Int32 ModifiedDateTimeStamp { get; set; }
        public Int32 Status { get; set; }
    }
}