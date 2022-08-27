using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Extensions;
using TourismSmartTransportation.Business.Interfaces.Mobile.Customer;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.MoMo;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.SearchModel.Shared.NotificationCollection;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Mobile.Customer
{
    public class BookingService : BaseService, IBookingService
    {
        private IFirebaseCloudMsgService _firebaseCloud;
        private INotificationCollectionService _notificationCollection;
        public BookingService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient, IFirebaseCloudMsgService firebaseCloud, INotificationCollectionService notificationCollection) : base(unitOfWork, blobServiceClient)
        {
            _firebaseCloud = firebaseCloud;
            _notificationCollection = notificationCollection;
        }

        /* public static string DecryptString(string cipherText)
         {
             string key = "b14pa58l8aee4133bhce2ea2315b1916";
             byte[] iv = new byte[16];
             byte[] buffer = Convert.FromBase64String(cipherText);

             using (Aes aes = Aes.Create())
             {
                 aes.Key = Encoding.UTF8.GetBytes(key);
                 aes.IV = iv;
                 ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                 using (MemoryStream memoryStream = new MemoryStream(buffer))
                 {
                     using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                     {
                         using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                         {
                             return streamReader.ReadToEnd();
                         }
                     }
                 }
             }
         }*/

        public async Task<PriceBookingViewModel> GetPrice(decimal distance, int seat)
        {
            var vehicleTypes = await _unitOfWork.VehicleTypeRepository.Query().Where(x => x.Seats == seat).ToListAsync();
            distance = distance / 1000;
            PriceBookingViewModel result = new PriceBookingViewModel();
            result.TotalPrice = 0;
            foreach (VehicleType vehicleType in vehicleTypes)
            {
                decimal priceTmp = 0;
                var price = await _unitOfWork.PriceOfBookingServiceRepository.Query().Where(x => x.VehicleTypeId.Equals(vehicleType.VehicleTypeId)).FirstOrDefaultAsync();
                if (distance > price.FixedDistance)
                {
                    priceTmp += price.FixedPrice;
                    priceTmp += price.PricePerKilometer * (distance - price.FixedDistance);
                }
                else
                {
                    priceTmp += price.FixedPrice;
                }
                if (priceTmp > result.TotalPrice)
                {
                    result = price.AsPriceBookingViewModel();
                    result.TotalPrice = priceTmp;
                }
            }

            return result;
        }

        public async Task<Response> RefundBooking(double amount, Guid customerId, string message = "")
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
            CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
            string mes = string.Format(elGR, "Quý khách được hoàn {0}% tiền đặt xe {1:N0} VNĐ ", message, amount);
            await _firebaseCloud.SendNotificationForRentingService(customer.RegistrationToken, "Hoàn tiền đặt xe", mes);
            SaveNotificationModel noti = new SaveNotificationModel()
            {
                CustomerId = customer.CustomerId.ToString(),
                CustomerFirstName = customer.FirstName,
                CustomerLastName = customer.LastName,
                Title = "Hoàn tiền đặt xe",
                Message = mes,
                Type = "Booking",
                Status = (int)NotificationStatus.Active
            };
            await _notificationCollection.SaveNotification(noti);
            return new()
            {
                StatusCode = 200,
                Message = "hoàn tiền thành công"
            };
        }
    }
}