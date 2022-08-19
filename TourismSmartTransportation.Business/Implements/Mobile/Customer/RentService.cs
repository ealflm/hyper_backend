using System;
using System.Collections.Generic;
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
using TourismSmartTransportation.Business.MoMo;
using TourismSmartTransportation.Business.SearchModel.Mobile.Customer;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;

namespace TourismSmartTransportation.Business.Implements.Mobile.Customer
{
    public class RentService : BaseService, IRentService
    {
        public RentService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public static string DecryptString(string cipherText)
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
        }

        public async Task<Response> CreateCustomerTrip(CustomerTripSearchModel model)
        {
            var customerTrip = new CustomerTrip()
            {
                CustomerTripId = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                CustomerId = model.CustomerId,
                VehicleId = new Guid(DecryptString(model.VehicleId)),
                RentDeadline = model.RentDeadline,
                Status = (int) CustomerTripStatus.Renting
            };
            var vehicle = await _unitOfWork.VehicleRepository.GetById(new Guid(DecryptString(model.VehicleId)));
            vehicle.Status = (int) VehicleStatus.Renting;
            _unitOfWork.VehicleRepository.Update(vehicle);
            await _unitOfWork.CustomerTripRepository.Add(customerTrip);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Tạo chuyến đi thành công"
            };
        }

        public async Task<PriceRentingViewModel> GetPrice(string id)
        {

            id = DecryptString(id);
            var vehicle = await _unitOfWork.VehicleRepository.GetById(new Guid(id));
            if (vehicle.Status ==1)
            {
                var price = await _unitOfWork.PriceOfRentingServiceRepository.GetById(vehicle.PriceRentingId.Value);
                var serviceType = await _unitOfWork.ServiceTypeRepository.Query().Where(x => x.Name.Contains(ServiceTypeDefaultData.RENT_SERVICE_NAME)).FirstOrDefaultAsync();
                var result = new PriceRentingViewModel()
                {
                    PriceOfRentingServiceId = price.PriceOfRentingServiceId,
                    MaxTime = price.MaxTime,
                    MinTime = price.MinTime,
                    PricePerHour = price.PricePerHour,
                    Status = price.Status,
                    VehicleName = vehicle.Name,
                    Color = vehicle.Color,
                    LicensePlates = vehicle.LicensePlates,
                    PricePerDay = price.FixedPrice,
                    PartnerId = vehicle.PartnerId,
                    ServiceTypeId = serviceType.ServiceTypeId
                };
                var dateNow = DateTime.Now;
                if (dateNow.DayOfWeek == DayOfWeek.Saturday || dateNow.DayOfWeek == DayOfWeek.Sunday)
                {
                    result.PricePerDay = price.WeekendPrice;
                }
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://holidays.abstractapi.com/v1/?api_key=ba043ce3a5274588ae52c2b1c4a6aebc&country=VN&year=" + dateNow.Year + "&month=" + dateNow.Month + "&day=" + dateNow.Day),
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    if (body.IndexOf("name") >= 0)
                    {
                        result.PricePerDay = price.HolidayPrice;
                    }
                }
                result.CategoryName = (await _unitOfWork.CategoryRepository.GetById(price.CategoryId)).Name;
                result.PublishYearName = (await _unitOfWork.PublishYearRepository.GetById(price.PublishYearId)).Name;
                return result;
            }
            return null;
        }

        public async Task<Response> ReturnVehicle(ReturnVehicleSearchModel model)
        {
            var customerTrip = await _unitOfWork.CustomerTripRepository.GetById(model.CustomerTripId);
            if (customerTrip == null)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Yêu cầu trả phương tiện thất bại"
                };
            }
            if (customerTrip.Status == (int)CustomerTripStatus.Renting)
            {
                customerTrip.Status = (int)CustomerTripStatus.Requesting;
                customerTrip.ReturnVehicleStationId = new Guid(DecryptString(model.RentStationId));
                _unitOfWork.CustomerTripRepository.Update(customerTrip);
                await _unitOfWork.SaveChangesAsync();
            }
            return new()
            {
                StatusCode = 200,
                Message = "Yêu cầu trả phương tiện thành công"
            };
        }
    }
}