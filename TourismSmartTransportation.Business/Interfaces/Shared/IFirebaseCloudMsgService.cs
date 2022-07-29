using System;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Shared;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;

namespace TourismSmartTransportation.Business.Interfaces.Shared
{
    public interface IFirebaseCloudMsgService
    {
        Task<Response> SaveRegistrationToken(FcmRegistrationTokenModel model, bool isPartner = false, bool isCustomer = false, bool isDriver = false);
        Task SendNotificationForRentingService(string clientToken, string title = "Thời hạn thuê xe của bạn sắp hết", string message = "Quý khách vừa nhận được thông báo từ hệ thống Hyper");
    }
}