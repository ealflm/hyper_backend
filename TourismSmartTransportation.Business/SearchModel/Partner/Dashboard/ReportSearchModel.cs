using System;
using System.ComponentModel.DataAnnotations;

namespace TourismSmartTransportation.Business.SearchModel.Partner.Dashboard
{
    public class ReportSearchModel
    {
        [Required]
        public Guid PartnerId { get; set; }
    }
}