using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Admin.ServiceTypeManagement
{
    public class ServiceTypeViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
    }
}
