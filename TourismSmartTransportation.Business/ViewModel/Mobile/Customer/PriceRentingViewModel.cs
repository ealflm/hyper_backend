using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Mobile.Customer
{
    public class PriceRentingViewModel
    {
        public Guid PriceOfRentingServiceId { get; set; }
        public string PublishYearName { get; set; }
        public string CategoryName { get; set; }
        public decimal MinTime { get; set; }
        public decimal MaxTime { get; set; }
        public decimal PricePerHour { get; set; }
        public decimal PricePerDay { get; set; }
        public string VehicleName { get; set; }
        public string LicensePlates { get; set; }
        public string Color { get; set; }
        public int Status { get; set; }
    }
}
