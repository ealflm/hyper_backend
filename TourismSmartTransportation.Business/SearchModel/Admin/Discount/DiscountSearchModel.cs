using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.SearchModel.Common;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Discount
{
    public class DiscountSearchModel : PagingSearchModel
    {
        [NotAllowedEmptyStringValidator]
        public string Title { get; set; }

        [NotAllowedEmptyStringValidator]
        public string Code { get; set; }

        [ValidStartDateTimeAttribute("TimeEnd", ErrorMessage = "Time start cannot be greater than Time end")]
        public DateTime? TimeStart { get; set; }

        public DateTime? TimeEnd { get; set; }

        [Range(0, 0.99)]
        public decimal? Value { get; set; }

        [Range(1, 2)]
        public int? Status { get; set; }
    }
}