using System;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PriceBookingServiceConfig
{
    public class PriceBookingServiceSearchModel : PriceBookingModel
    {
        public int? Status { get; set; }
    }
}