using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.SearchModel.Mobile.Customer
{
    public class FeedbackForDriverSearchModel
    {
        [Required]
        public Guid CustomerTripId { get; set; }
        [Required]
        public int Rate { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
