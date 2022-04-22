using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Discount
{
    public class CreateDiscountModel
    {
        [NotAllowedEmptyStringValidator]
        public string Title { get; set; }

        [NotAllowedEmptyStringValidator]
        public string Code { get; set; }

        public string Description { get; set; }

        [ValidStartDateTimeAttribute("TimeEnd", ErrorMessage = "Time start cannot be greater than Time end")]
        public DateTime? TimeStart { get; set; }

        public DateTime? TimeEnd { get; set; }

        public string PhotoUrls { get; set; }

        [DiscountValueValidator]
        public decimal? Value { get; set; }

        [CheckStatusRecordValidator]
        public int Status { get; set; }
    }
}