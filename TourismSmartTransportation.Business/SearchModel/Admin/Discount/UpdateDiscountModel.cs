using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Discount
{
    public class UpdateDiscountModel : FileViewModel
    {
        public string Title { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        [ValidStartDateTimeAttribute("TimeEnd", ErrorMessage = "Time start cannot be greater than Time end")]
        public DateTime? TimeStart { get; set; }

        public DateTime? TimeEnd { get; set; }

        public string PhotoUrls { get; set; }

        [DiscountValueValidator]
        public decimal? Value { get; set; }

        public int? Status { get; set; }
    }
}