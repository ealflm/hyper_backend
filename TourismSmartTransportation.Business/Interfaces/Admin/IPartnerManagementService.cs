using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Admin.PartnerManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.PartnerManagement;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IPartnerManagementService
    {
        Task<SearchResultViewModel<PartnerViewModel>> SearchPartner(PartnerSearchModel model);
        Task<PartnerViewModel> GetPartner(Guid id);
        Task<bool> AddPartner(AddPartnerModel model);
        Task<bool> UpdatePartner(Guid id, UpdatePartnerModel model);
        Task<bool> DeletePartner(Guid id);
    }
}