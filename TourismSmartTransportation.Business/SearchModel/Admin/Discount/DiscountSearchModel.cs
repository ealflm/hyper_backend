using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.SearchModel.Common;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Discount
{
    public class DiscountSearchModel : PagingSearchModel
    {
        public string Title { get; set; }

        public string Code { get; set; }

        [ValidStartDateTimeAttribute("TimeEnd", ErrorMessage = "Time start cannot be greater than Time end")]
        public DateTime? TimeStart { get; set; }

        public DateTime? TimeEnd { get; set; }

        [DiscountValueValidator]
        public decimal? Value { get; set; }

        public int? Status { get; set; }
    }
}