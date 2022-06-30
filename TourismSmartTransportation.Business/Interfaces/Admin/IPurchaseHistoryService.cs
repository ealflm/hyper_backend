using System;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.ViewModel.Admin.PurchaseHistory;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IPurchaseHistoryService
    {
        Task<SearchResultViewModel<OrderViewModel>> GetOrder(Guid customerId);
        // Task<SearchResultViewModel<OrderDetailOfPackageViewModel>> GetOrderDetailOfPackage(Guid orderId);
        // Task<SearchResultViewModel<OrderDetailOfBookingServiceViewModel>> GetOrderDetailOfBookingService(Guid orderId);
        // Task<SearchResultViewModel<OrderDetailOfBusServiceViewModel>> GetOrderDetailOfBusService(Guid orderId);
        // Task<SearchResultViewModel<OrderDetailOfRentingServiceViewModel>> GetOrderDetailOfRentingService(Guid orderId);
        Task<Object> GetOrderDetail(Guid orderId);
        Task<SearchResultViewModel<TransactionViewModel>> GetTransaction(Guid orderId);

    }
}
