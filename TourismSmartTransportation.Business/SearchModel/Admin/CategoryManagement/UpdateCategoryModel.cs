using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.Validation;

namespace TourismSmartTransportation.Business.SearchModel.Admin.CategoryManagement
{
    public class UpdateCategoryModel
    {
        [NotAllowedEmptyStringValidator]
        public string Name { get; set; }

        [NotAllowedEmptyStringValidator]
        public string Description { get; set; }

        public int? Status { get; set; }
    }
}
