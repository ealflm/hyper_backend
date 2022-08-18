using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Mobile.Customer
{
    public class FeedbackForVehicleViewModel
    {
        public Guid CustomerTripId { get; set; }
        public int Rate { get; set; }
        public string Content { get; set; }

        public Guid FeedbackVehicleId { get; set; }
    }
}
