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
    public interface IDepositService
    {
        Task<DepositViewModel> GetOrderId(DepositSearchModel model);
        Task<Response> GetOrderStatus(Guid id);
    }
}