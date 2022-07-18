using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.SearchModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Package
{
    public class PackageCustomerModel : PagingSearchModel
    {
        [Required]
        public Guid CustomerId { get; set; }
    }
}