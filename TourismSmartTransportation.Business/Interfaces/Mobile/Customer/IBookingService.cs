using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Admin.StationManagement;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer.BusTrip;
using TourismSmartTransportation.Business.ViewModel.Partner.RouteManagement;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Interfaces.Mobile.Customer
{
    public interface IBookingService
    {
        Task<PriceBookingViewModel> GetPrice(decimal distance, int seat);
    }
}
