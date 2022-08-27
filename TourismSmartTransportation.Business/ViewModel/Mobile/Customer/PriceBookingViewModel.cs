using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Mobile.Customer
{
    public class PriceBookingViewModel
    {
        public Guid PriceOfBookingServiceId { get; set; }
        public Guid VehicleTypeId { get; set; }
        public decimal FixedPrice { get; set; }
        public decimal FixedDistance { get; set; }
        public decimal PricePerKilometer { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PriceAfterDiscount { get; set; }
    }
}
