using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TourismSmartTransportation.Data.MongoCollections.Vehicle
{
    public class AddVehicleCollectionModel
    {

        public Guid VehicleId { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
    }
}