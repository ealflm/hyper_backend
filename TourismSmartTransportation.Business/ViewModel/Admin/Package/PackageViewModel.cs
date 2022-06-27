using System;
using System.Collections.Generic;
using TourismSmartTransportation.Business.ViewModel.Admin.PackageItem;

namespace TourismSmartTransportation.Business.ViewModel.Admin.Package
{
    public class PackageViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PeopleQuanitty { get; set; }
        public int Duration { get; set; }
        public string PhotoUrl { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PromotedTitle { get; set; }
        public List<PackageItemViewModel> PackageItems { get; set; }
        public int Status { get; set; }
    }
}