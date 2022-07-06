using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Discount
{
    public class UpdateDiscountModel : FileViewModel
    {
        public Guid? ServiceTypeId { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        [ValidStartDateTimeAttribute("TimeEnd", ErrorMessage = "Thời gian bắt đầu không được lớn hơn thời gian kết thúc")]
        public DateTime? TimeStart { get; set; }

        public DateTime? TimeEnd { get; set; }

        public string PhotoUrls { get; set; }

        [DiscountValueValidator]
        public decimal? Value { get; set; }

        public int? Status { get; set; }
    }
}