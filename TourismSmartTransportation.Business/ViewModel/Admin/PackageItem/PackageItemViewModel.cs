using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.PackageItem
{
    public class PackageItemViewModel
    {
        public Guid PackageId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public string Name { get; set; }
        public decimal Limit { get; set; }
        public decimal Value { get; set; }
        public int Status { get; set; }
    }
}
