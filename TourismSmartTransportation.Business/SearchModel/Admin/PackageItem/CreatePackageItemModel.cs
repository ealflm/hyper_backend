using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.PackageItem
{
    // [ModelBinder(BinderType = typeof(MetadataValueModelBinder))]
    public class CreatePackageItemModel
    {
        // public Guid Id { get; set; }
        public Guid PackageId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public string Name { get; set; }
        public decimal Limit { get; set; }
        public decimal Value { get; set; }
        public int? Status { get; set; }
    }
}
