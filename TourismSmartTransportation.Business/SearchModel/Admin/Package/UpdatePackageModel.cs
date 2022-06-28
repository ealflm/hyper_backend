using System;
using System.Collections.Generic;
using TourismSmartTransportation.Business.ViewModel.Admin.PackageItem;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Package
{
    public class UpdatePackageModel : FileViewModel
    {
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public string PromotedTitle { get; set; }
        public List<UpdatePackageItemModel> PackageItems { get; set; }
        public int? Status { get; set; }
    }
}