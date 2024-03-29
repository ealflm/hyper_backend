﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;

namespace TourismSmartTransportation.Business.Interfaces.Mobile.Customer
{
    public interface IRentService
    {
        Task<PriceRentingViewModel> GetPrice(string id);
        Task<PriceRentingViewModel> GetPriceExtend(Guid customerTripId);
        Task<Response> CreateCustomerTrip(CustomerTripSearchModel model);
        Task<Response> ReturnVehicle(ReturnVehicleSearchModel model);
        Task<Response> CheckMergeOrder(int time, Guid customerTripId);
        Task<Response> ExtendRentingOrder(ExtendOrderSearchModel model);

    }
}
