using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.Discount
{
    public class DiscountViewModel
    {
        public Guid Id { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string PhotoUrl { get; set; }
        public decimal Value { get; set; }
        public int Status { get; set; }
        public string Code { get; set; }
    }
}