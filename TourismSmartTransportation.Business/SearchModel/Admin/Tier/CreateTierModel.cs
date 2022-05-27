using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.ViewModel.Admin.Package;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Tier
{
    public class CreateTierModel : FileViewModel
    {
        [Required]
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public List<PackageViewModel> PackageList { get; set; }
        [Required]
        public string PromotedTitle { get; set; }
    }
}