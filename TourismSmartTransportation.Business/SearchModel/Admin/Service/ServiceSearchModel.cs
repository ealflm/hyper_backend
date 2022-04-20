using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.SearchModel.Common;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.Service
{
    public class ServiceSearchModel : PagingSearchModel
    {
        [NotAllowedEmptyStringValidator]
        public string Title { get; set; }

        [NotAllowedEmptyStringValidator]
        public string Description { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Value should be greater than or equal to 1")]
        public decimal? Price { get; set; }

        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }

        [Range(1, 2)]
        public int? Status { get; set; }
    }
}