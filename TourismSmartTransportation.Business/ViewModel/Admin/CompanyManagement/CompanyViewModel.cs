using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourismSmartTransportation.Business.ViewModel.Admin.CompanyManagement
{
    public class CompanyViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string PhotoUrl { get; set; }
        public int Status { get; set; }
    }
}