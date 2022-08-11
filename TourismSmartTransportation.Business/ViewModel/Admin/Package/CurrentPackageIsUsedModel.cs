using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.Package
{
    public class CurrentPackageIsUsedModel
    {
        public Guid PackageId { get; set; }
        public String PackageName { get; set; }
        public DateTime PackageExpire { get; set; }
        public decimal PackagePrice { get; set; }
        public string PackagePhotoUrl { get; set; }
        public decimal CurrentDistances { get; set; }
        public decimal LimitDistances { get; set; }
        public decimal CurrentCardSwipes { get; set; }
        public decimal LimitCardSwipes { get; set; }
        public decimal CurrentNumberOfTrips { get; set; }
        public decimal LimitNumberOfTrips { get; set; }
        public decimal DiscountValueTrip { get; set; }
    }
}