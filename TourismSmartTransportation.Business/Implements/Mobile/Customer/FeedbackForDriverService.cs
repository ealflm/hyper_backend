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
    public class FeedbackForDriverService : BaseService, IFeedbackForDriverService
    {
        public FeedbackForDriverService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<Response> CreateFeedback(FeedbackForDriverSearchModel model)
        {
            var customerTrip = await _unitOfWork.CustomerTripRepository.GetById(model.CustomerTripId);
            var driver = await _unitOfWork.DriverRepository.Query().Where(x => x.VehicleId.Equals(customerTrip.VehicleId)).FirstOrDefaultAsync();
            var feedback = new FeedbackForDriver()
            {
                CustomerTripId = model.CustomerTripId,
                DriverId= driver.DriverId,
                Rate= model.Rate,
                Content= model.Content,
                FeedbackForDriverId= Guid.NewGuid(),
                Status= 1
            };
            await _unitOfWork.FeedbackForDriverRepository.Add(feedback);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 200,
                Message = "Đánh giá thành công"
            };
        }

        public async Task<FeedbackForDriverViewModel> GetFeedBackByCustomerTripId(Guid customerTripId)
        {
            var feedback = await _unitOfWork.FeedbackForDriverRepository.Query().Where(x => x.CustomerTripId.Equals(customerTripId)).FirstOrDefaultAsync();
            return feedback.AsFeedbackForDriverViewModel();
        }
    }
}