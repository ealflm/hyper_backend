using System;

namespace TourismSmartTransportation.Business.ViewModel.Admin.PriceRentingServiceViewModel
{
    public class PriceOfRentingServiceViewModel
    {
        public Guid Id { get; set; }
        public Guid PublishYearId { get; set; }
        public string PublishYearName { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal MinTime { get; set; }
        public decimal MaxTime { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal FixedPrice { get; set; }
        public decimal WeekendPrice { get; set; }
        public decimal HolidayPrice { get; set; }
        public int Status { get; set; }
    }
}