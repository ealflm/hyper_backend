using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Validation;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.ViewModel.Admin.CompanyManagement
{
    public class AddCompanyViewModel: FileViewModel
    {
        [NotAllowedEmptyStringValidator]
        public string Name { get; set; }
        [NotAllowedEmptyStringValidator]
        public string UserName { get; set; }
        [NotAllowedEmptyStringValidator]
        public string Password { get; set; }
        [NotAllowedEmptyStringValidator]
        public string Address { get; set; }
        [Range(1, 2)]
        public int? Status { get; set; }
    }
}