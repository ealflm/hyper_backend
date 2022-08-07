using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.MoMo;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Admin.PurchaseHistory;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Mobile.Customer
{
    public class PurchaseHistoryService : BaseService, IPurchaseHistoryService
    {
        public PurchaseHistoryService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<PurchaseHistoryViewModel> GetPurchase(Guid customerId)
        {
            var orders = await _unitOfWork.OrderRepository.Query().Where(x => x.CustomerId.Equals(customerId)).OrderByDescending(x => x.CreatedDate).Select(x=> x.AsOrderViewModel()).ToListAsync();
            var wallet = await _unitOfWork.WalletRepository.Query().Where(x => x.CustomerId.Equals(customerId)).FirstOrDefaultAsync();
            var transaction = await _unitOfWork.TransactionRepository.Query().Where(x => x.WalletId.Equals(wallet.WalletId)).OrderByDescending(x => x.CreatedDate).Select(x=> x.AsTransactionViewModel()).ToListAsync();
            var customerTrip = await _unitOfWork.CustomerTripRepository.Query().Where(x => x.CustomerId.Equals(customerId)).OrderByDescending(x => x.CreatedDate).Select(x=> x.AsCustomerTripViewModel()).ToListAsync();
            foreach(OrderViewModel x in orders)
            {
                if (x.ServiceTypeId != null)
                {
                    x.ServiceTypeName = (await _unitOfWork.ServiceTypeRepository.GetById(x.ServiceTypeId.Value)).Name;
                }
            }
            foreach(CustomerTripViewModel x in customerTrip)
            {
                var vehicle = await _unitOfWork.VehicleRepository.GetById(x.VehicleId);
                var servicetype = await _unitOfWork.ServiceTypeRepository.GetById(vehicle.ServiceTypeId);
                x.VehicleName = vehicle.Name;
                x.LicensePlates = vehicle.LicensePlates;
                x.ServiceTypeName = servicetype.Name;
            }
            var purchaseHistory = new PurchaseHistoryViewModel()
            {
                Orders = orders,
                Transactions = transaction,
                CustomerTrips = customerTrip
            };
            return purchaseHistory;
        }
    }
}