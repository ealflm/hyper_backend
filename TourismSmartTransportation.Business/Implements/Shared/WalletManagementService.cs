using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.ViewModel.Shared;
using TourismSmartTransportation.Data.Interfaces;

namespace TourismSmartTransportation.Business.Implements.Shared
{
    public class WalletManagementService : BaseService, IWalletManagementService
    {
        public WalletManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }
        public async Task<WalletViewModel> GetWallet(Guid? id)
        {
            if (id == null)
            {
                return await _unitOfWork.WalletRepository
                        .Query()
                        .Where(x => x.CustomerId == null && x.PartnerId == null)
                        .Select(x => x.AsWalletViewModel())
                        .SingleOrDefaultAsync();
            }

            return await _unitOfWork.WalletRepository
                        .Query()
                        .Where(x => x.CustomerId == id.Value || x.PartnerId == id.Value)
                        .Select(x => x.AsWalletViewModel())
                        .SingleOrDefaultAsync();
        }
    }
}