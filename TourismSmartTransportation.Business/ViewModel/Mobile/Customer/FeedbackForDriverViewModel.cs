using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Mobile.Customer
{
     public class FeedbackForDriverViewModel
    {
        public Guid FeedbackForDriverId { get; set; }
        public Guid CustomerTripId { get; set; }
        public Guid DriverId { get; set; }
        public int Rate { get; set; }
        public string Content { get; set; }
    }
}
