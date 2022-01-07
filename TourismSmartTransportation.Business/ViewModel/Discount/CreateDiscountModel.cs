using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.ViewModel.Admin.Discount
{
    public class CreateDiscountModel
    {
        [Required]
        [NotAllowedEmptyStringValidator]
        public string Title { get; set; }

        [Required]
        [NotAllowedEmptyStringValidator]
        public string Code { get; set; }

        [Required]
        [ValidStartDateTimeAttribute("TimeEnd", ErrorMessage = "Time start cannot be greater than Time end")]
        public DateTime TimeStart { get; set; }

        [Required]
        public DateTime TimeEnd { get; set; }

        [Required]
        public decimal Value { get; set; }

        [Range(1, 2)]
        public int? Status { get; set; }
    }
}