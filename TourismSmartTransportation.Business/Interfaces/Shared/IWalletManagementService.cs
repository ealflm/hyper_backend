using System;
using System.Threading.Tasks;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.ViewModel.Shared;

namespace TourismSmartTransportation.Business.Interfaces.Shared
{
    public interface IWalletManagementService
    {
        Task<WalletViewModel> GetWallet(Guid? id);
    }
}