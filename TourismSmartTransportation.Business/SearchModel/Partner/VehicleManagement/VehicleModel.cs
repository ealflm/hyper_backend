using System;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement
{
    public class VehicleModel
    {
        public virtual Guid? ServiceTypeId { get; set; }

        public virtual Guid? VehicleTypeId { get; set; }

        public virtual Guid? RentStationId { get; set; }

        public virtual Guid? PartnerId { get; set; }

        // public virtual Guid? PriceRentingId { get; set; }
        public virtual Guid? CategoryId { get; set; }

        public virtual Guid? PublishYearId { get; set; }

        public virtual string Name { get; set; }

        public virtual string LicensePlates { get; set; }

        public virtual string Color { get; set; }
    }
}