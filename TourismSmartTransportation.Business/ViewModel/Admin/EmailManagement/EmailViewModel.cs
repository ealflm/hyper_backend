using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Admin.EmailManagement
{
    public class EmailViewModel
    {
        public string Subject { get; set; }
        public string ToAddress { get; set; }
        public string Body { get; set; }
    }
}
