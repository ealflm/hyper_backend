using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Discount
{
    public class CreateDiscountModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [ValidStartDateTimeAttribute("TimeEnd", ErrorMessage = "Time start cannot be greater than Time end")]
        public DateTime TimeStart { get; set; }

        [Required]
        public DateTime TimeEnd { get; set; }

        public string PhotoUrls { get; set; }

        [Required]
        [DiscountValueValidator]
        public decimal Value { get; set; }
    }
}