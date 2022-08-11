using System;
using System.ComponentModel.DataAnnotations;

namespace TourismSmartTransportation.Business.SearchModel.Mobile.Customer
{
    public class BuyPackageModel
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid PackageId { get; set; }
    }
}