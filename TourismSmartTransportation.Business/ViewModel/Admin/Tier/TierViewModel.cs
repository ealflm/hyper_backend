using System;
using System.Collections.Generic;
using TourismSmartTransportation.Business.ViewModel.Admin.Package;

namespace TourismSmartTransportation.Business.ViewModel.Admin.Tier
{
    public class TierViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PromotedTitle { get; set; }
        public List<PackageViewModel> PackageList { get; set; }
        public int Status { get; set; }
    }
}