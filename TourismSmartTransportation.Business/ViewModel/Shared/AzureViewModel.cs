using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Shared
{
    public class AzureViewModel
    {
        public string GrantType { get; set; }
        public string Resource { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string DataSetId { get; set; }
        public string ReportId { get; set; }
    }
}
