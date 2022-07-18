using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;
using TourismSmartTransportation.Data.Interfaces;

namespace TourismSmartTransportation.Business.Implements.Mobile.Customer
{
    public class WalletManagementService : BaseService, IWalletManagementService
    {
        public WalletManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }
        public async Task<WalletViewModel> GetWalletByCustomer(Guid customerId)
        {
            var wallet = await _unitOfWork.WalletRepository
                        .Query()
                        .Where(x => x.CustomerId == customerId)
                        .Select(x => x.AsWalletViewModel())
                        .SingleOrDefaultAsync();
            return wallet;
        }
    }
}