﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Partner.RentStationManagement;
using TourismSmartTransportation.Business.ViewModel.Admin.CustomerManagement;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Partner.RentStationManagement;

namespace TourismSmartTransportation.Business.Interfaces.Company
{
    public interface IRentStationManagementService
    {
        Task<SearchResultViewModel<RentStationViewModel>> SearchRentStation(RentStationSearchModel model);
        Task<RentStationViewModel> GetRentStation(Guid id);
        Task<Response> AddRentStation(AddRentStationModel model);
        Task<Response> UpdateRentStaion(Guid id, UpdateRentStation model);
        Task<Response> DeleteRentStation(Guid id);
    }
}
