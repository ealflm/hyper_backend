using System;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement
{
    public class UpdateVehicleModel : VehicleModel
    {
        [NotAllowedEmptyStringValidator]
        public override string Name { get; set; }

        [NotAllowedEmptyStringValidator]
        public override string LicensePlates { get; set; }

        [NotAllowedEmptyStringValidator]
        public override string Color { get; set; }

        public int? Status { get; set; }

        public int? IsRunning { get; set; }
    }
}