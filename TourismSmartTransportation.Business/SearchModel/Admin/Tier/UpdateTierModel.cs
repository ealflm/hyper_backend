using System;
using System.Collections.Generic;
using TourismSmartTransportation.Business.ViewModel.Admin.Package;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Tier
{
    public class UpdateTierModel : FileViewModel
    {
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public string PromotedTitle { get; set; }
        public List<UpdatePackageModel> PackageList { get; set; }
        public int? Status { get; set; }
    }
}