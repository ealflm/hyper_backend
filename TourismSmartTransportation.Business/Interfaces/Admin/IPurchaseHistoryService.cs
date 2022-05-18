using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.ViewModel.Admin.PurchaseHistory;
using TourismSmartTransportation.Business.ViewModel.Common;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IPurchaseHistoryService
    {
        Task<SearchResultViewModel<OrderViewModel>> GetOrder(string customerId);
        Task<SearchResultViewModel<OrderDetailViewModel>> GetOrderDetail(string orderId);
        Task<SearchResultViewModel<PaymentViewModel>> GetPayment(string orderId);
        Task<SearchResultViewModel<TransactionViewModel>> GetTransaction(string paymentId);

    }
}
