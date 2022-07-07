using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TourismSmartTransportation.Business.Validation;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.SearchModel.Admin.PartnerManagement
{
    public class UpdatePartnerByAdmin : UpdatePartnerModel
    {
        public List<Guid?> AddServiceTypeIdList { get; set; }
        public List<Guid?> DeleteServiceTypeIdList { get; set; }
    }
}