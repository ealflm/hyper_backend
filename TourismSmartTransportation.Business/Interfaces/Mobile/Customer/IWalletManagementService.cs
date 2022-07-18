using System;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;

namespace TourismSmartTransportation.Business.Interfaces.Mobile.Customer
{
    public interface IWalletManagementService
    {
        Task<WalletViewModel> GetWalletByCustomer(Guid customerId);
    }
}