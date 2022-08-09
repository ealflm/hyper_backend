using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.ViewModel.Admin.PackageItem;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Package
{
    public class CreatePackageModel : FileViewModel
    {
        [Required]
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public List<CreatePackageItemModel> PackageItems { get; set; }
        [Required]
        public string PromotedTitle { get; set; }
        [Required]
        public int PeopleQuanitty { get; set; }
        [Required]
        public int Duration { get; set; }
    }
}