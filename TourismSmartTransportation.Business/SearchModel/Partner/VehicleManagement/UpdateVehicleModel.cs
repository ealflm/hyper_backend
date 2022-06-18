using System;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement
{
    public class UpdateVehicleModel
    {
        public Guid? ServiceTypeId { get; set; }
        public Guid? VehicleTypeId { get; set; }
        public Guid? RentStationId { get; set; }
        public string Name { get; set; }
        public string LicensePlates { get; set; }
        public string Color { get; set; }
        public int? Status { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? PublishYearId { get; set; }
        public int? IsRunning { get; set; }
    }
}