using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.SearchModel.Partner.TripManagement
{
    public class CopyTripModel
    {
        public string FromWeek { get; set; }
        public string ToWeek { get; set; }
        public Guid PartnerId { get; set; }

    }
}
