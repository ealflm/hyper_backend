using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.Order;
using TourismSmartTransportation.Business.SearchModel.Admin.PurchaseManagement.OrderDetail;

namespace TourismSmartTransportation.Business.Interfaces.Admin
{
    public interface IOrderHelpersService
    {
        Task<Response> CreateOrder(CreateOrderModel order, OrderDetailsInfo orderDetailsInfo);
        Task<Response> MakeOrderTest(MakeOrderTestModel model);
    }
}