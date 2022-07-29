using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using FirebaseAdmin.Messaging;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.SearchModel.Shared;
using TourismSmartTransportation.Business.ViewModel.Mobile.Customer;
using TourismSmartTransportation.Data.Interfaces;

namespace TourismSmartTransportation.Business.Implements.Shared
{
    public class FirebaseCloudMsgService : BaseService, IFirebaseCloudMsgService
    {
        public FirebaseCloudMsgService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        /// <summary>
        /// Save registration token for retrieve notificaiton
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isPartner"></param>
        /// <param name="isCustomer"></param>
        /// <param name="isDriver"></param>
        /// <returns></returns>
        public async Task<Response> SaveRegistrationToken(FcmRegistrationTokenModel model, bool isPartner = false, bool isCustomer = false, bool isDriver = false)
        {
            if (isPartner)
            {
                var partner = await _unitOfWork.PartnerRepository.GetById(model.EntityId);
                if (partner == null)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Người dùng không hợp lệ!"
                    };
                }

                partner.RegistrationToken = model.RegistrationToken;
                _unitOfWork.PartnerRepository.Update(partner);

            }
            else if (isCustomer)
            {
                var customer = await _unitOfWork.CustomerRepository.GetById(model.EntityId);
                if (customer == null)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Người dùng không hợp lệ!"
                    };
                }

                customer.RegistrationToken = model.RegistrationToken;
                _unitOfWork.CustomerRepository.Update(customer);
            }
            else if (isDriver)
            {
                var driver = await _unitOfWork.DriverRepository.GetById(model.EntityId);
                if (driver == null)
                {
                    return new()
                    {
                        StatusCode = 400,
                        Message = "Người dùng không hợp lệ!"
                    };
                }

                driver.RegistrationToken = model.RegistrationToken;
                _unitOfWork.DriverRepository.Update(driver);
            }
            else
            {
                return new()
                {
                    StatusCode = 500,
                    Message = "Lỗi logic hệ thống"
                };
            }

            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Lưu thành công!"
            };
        }

        public async Task SendNotificationForRentingService(string clientToken, string title, string content)
        {
            var message = new Message()
            {
                Token = clientToken,
                Notification = new Notification()
                {
                    Title = title,
                    Body = content
                },
            };

            //Send message to device correspond 
            var response =
                await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }
}