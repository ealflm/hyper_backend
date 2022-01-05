using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.SearchModel.Admin.StationManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IStationManagementService
    {
        Task<SearchStationResultViewModel> SearchStation(StationSearchModel model = null);
        Task<StationViewModel> GetStation(Guid id);
        Task<bool> AddStation(AddStationViewModel model);
        Task<bool> UpdateStation(Guid id, AddStationViewModel model);
        Task<bool> DeleteStation(Guid id);
    }
}
