using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;
using TourismSmartTransportation.Business.Extensions;
using Azure.Storage.Blobs;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.ViewModel.Admin.CardManagement;
using TourismSmartTransportation.Business.SearchModel.Admin.CardManagement;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class CardManagementService : BaseService, ICardManagementService
    {
        public CardManagementService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient) : base(unitOfWork, blobServiceClient)
        {
        }

        public async Task<Response> Create(string uid)
        {
            var isExistCode = await _unitOfWork.CardRepository.Query().AnyAsync(x => x.Uid.Equals(uid));
            if (isExistCode)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Thẻ đã tồn tại!"
                };
            }
            var card = new Card()
            {
                CardId = Guid.NewGuid(),
                Uid = uid,
                Status = 1
            };

            await _unitOfWork.CardRepository.Add(card);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Tạo mới thẻ thành công!"
            };
        }

        public async Task<Response> Delete(Guid id)
        {
            var entity = await _unitOfWork.CardRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            entity.Status = 0;
            _unitOfWork.CardRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật trạng thái thành công!"
            };
        }

        public async Task<CardViewModel> GetById(Guid id)
        {
            var entity = await _unitOfWork.CardRepository.GetById(id);
            var model = entity.AsCardViewModel();
            if (model.CustomerId != null)
            {
                var customer = await _unitOfWork.CustomerRepository.GetById(model.CustomerId.Value);
                model.CustomerName = customer.FirstName + " " + customer.LastName;
                model.PhotoUrl = customer.PhotoUrl;
            }
            return model;

        }

        public async Task<List<CardViewModel>> Search(CardSearchModel model)
        {
            var entity = await _unitOfWork.CardRepository.Query()
                            .Where(x => model.Uid == null || x.Uid.Contains(model.Uid))
                            .Where(x => model.CustomerId == null || x.CustomerId == model.CustomerId.Value)
                            .Where(x => model.Status == null || x.Status == model.Status.Value)
                            .Select(x => x.AsCardViewModel())
                            .ToListAsync();
            foreach (CardViewModel x in entity)
            {

                if (x.CustomerId != null)
                {
                    var customer = await _unitOfWork.CustomerRepository.GetById(x.CustomerId.Value);
                    x.CustomerName = customer.FirstName + " " + customer.LastName;
                    x.PhotoUrl = customer.PhotoUrl;
                }

            }
            return entity;

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
        public async Task<Response> Update(Guid id, UpdateCardModel model)
        {
            var isExistCode = await _unitOfWork.CardRepository.Query().AnyAsync(x => x.Uid.Equals(model.Uid));
            if (isExistCode)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Thẻ đã tồn tại!"
                };
            }
            var entity = await _unitOfWork.CardRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            entity.CustomerId = UpdateTypeOfNotNullAbleObject<Guid>(entity.CustomerId, model.CustomerId);
            entity.Uid = UpdateTypeOfNullAbleObject<string>(entity.Uid, model.Uid);
            entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
            _unitOfWork.CardRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }


        public async Task<Response> CustomerMatch(UpdateCardModel model)
        {
            model.Uid = DecryptString(model.Uid.Substring(1));
            var entity = await _unitOfWork.CardRepository.Query().Where(x=> x.Uid.Equals(model.Uid)).FirstOrDefaultAsync();
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy!"
                };
            }
            if (model.CustomerId != null)
            {
                entity.CustomerId = model.CustomerId;
            }
            else
            {
                entity.CustomerId = null;
            }
            entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);
            _unitOfWork.CardRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật thành công!"
            };
        }
    }
}