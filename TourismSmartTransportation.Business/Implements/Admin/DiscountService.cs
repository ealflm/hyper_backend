using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.SearchModel.Admin.Discount;
using TourismSmartTransportation.Business.ViewModel.Admin.Discount;
using TourismSmartTransportation.Business.ViewModel.Common;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.Models;
using TourismSmartTransportation.Business.Extensions;
using Azure.Storage.Blobs;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.SearchModel.Shared.NotificationCollection;

namespace TourismSmartTransportation.Business.Implements.Admin
{
    public class DiscountService : BaseService, IDiscountService
    {
        private readonly IFirebaseCloudMsgService _firebaseService;
        private readonly INotificationCollectionService _notiService;
        public DiscountService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient,
                                IFirebaseCloudMsgService firebaseService, INotificationCollectionService notiService) : base(unitOfWork, blobServiceClient)
        {
            _firebaseService = firebaseService;
            _notiService = notiService;
        }

        public async Task<Response> CreateDiscount(CreateDiscountModel model)
        {
            // var serviceType = await _unitOfWork.ServiceTypeRepository.GetById(model.ServiceTypeId);
            // if (serviceType == null)
            // {
            //     return new()
            //     {
            //         StatusCode = 400,
            //         Message = "Loại dịch vụ không tồn tại!"
            //     };
            // }

            string discountCode = GenerateTextAuto(lengthOfString: 8, specialText: false);
            while (true)
            {
                var isExist = await _unitOfWork.DiscountRepository
                            .Query()
                            .AnyAsync(x => x.Code == discountCode);

                if (!isExist)
                {
                    break;
                }
                discountCode = GenerateTextAuto(lengthOfString: 8, specialText: false);
            }


            var entity = new Discount()
            {
                DiscountId = Guid.NewGuid(),
                ServiceTypeId = model.ServiceTypeId,
                Title = model.Title,
                Description = model.Description,
                TimeStart = model.TimeStart,
                TimeEnd = model.TimeEnd,
                PhotoUrl = await UploadFile(model.UploadFile, Container.Admin),
                Value = model.Value,
                Status = (int)DiscountStatus.UnSent,
                Code = discountCode
            };

            await _unitOfWork.DiscountRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Tạo mới khuyến mãi thành công!"
            };
        }

        public async Task<Response> DeleteDiscount(Guid id)
        {
            var entity = await _unitOfWork.DiscountRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy mã giảm giá này!"
                };
            }

            entity.Status = (int)DiscountStatus.Disabled;
            _unitOfWork.DiscountRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Mã giảm giá đã bị vô hiệu hóa"
            };

        }

        public async Task<DiscountViewModel> GetDiscount(Guid id)
        {
            var entity = await _unitOfWork.DiscountRepository.GetById(id);
            if (entity == null)
            {
                return null;
            }
            return entity.AsDiscountViewModel();
        }

        // public async Task<SearchResultViewModel<DiscountViewModel>> SearchDiscount(DiscountSearchModel model)
        // {
        //     Func<object, SearchResultViewModel<DiscountViewModel>> returnFunc = (param) =>
        //     {
        //         DiscountSearchModel model = (DiscountSearchModel)param;
        //         var source = _unitOfWork.DiscountRepository
        //                     .FindAsNoTracking()
        //                     .FilterFunc(model);
        //         var totalItems = source.Count();
        //         var items = source
        //                         .OrderByCustomFunc(model.SortBy)
        //                         .PaginateFunc(model.PageIndex, model.ItemsPerPage)
        //                         .Select(item => item.AsDiscountViewModel())
        //                         .ToList();
        //         var pageSize = GetPageSize(model.ItemsPerPage, totalItems);
        //         return new SearchResultViewModel<DiscountViewModel>(items, pageSize, totalItems);
        //     };

        //     Task<SearchResultViewModel<DiscountViewModel>> task = new Task<SearchResultViewModel<DiscountViewModel>>(returnFunc, model);
        //     task.Start();
        //     return await task;

        // }

        public async Task<SearchResultViewModel<DiscountViewModel>> SearchDiscount(DiscountSearchModel model)
        {
            var discount = await _unitOfWork.DiscountRepository.Query()
                .Where(x => model.Title == null || x.Title.Contains(model.Title))
                .Where(x => model.TimeStart == null || DateTime.Compare(x.TimeStart, model.TimeStart.Value) >= 0)
                .Where(x => model.TimeEnd == null || DateTime.Compare(x.TimeEnd, model.TimeEnd.Value) <= 0)
                .Where(x => model.Value == null || x.Value == model.Value.Value)
                .Where(x => model.Status == null || x.Status == model.Status.Value)
                .OrderBy(x => x.Value)
                .Select(x => x.AsDiscountViewModel())
                .ToListAsync();
            var listAfterSorting = GetListAfterSorting(discount, model.SortBy);
            var totalRecord = GetTotalRecord(listAfterSorting, model.ItemsPerPage, model.PageIndex);
            var listItemsAfterPaging = GetListAfterPaging(listAfterSorting, model.ItemsPerPage, model.PageIndex, totalRecord);
            SearchResultViewModel<DiscountViewModel> result = null;
            result = new SearchResultViewModel<DiscountViewModel>()
            {
                Items = listItemsAfterPaging,
                PageSize = GetPageSize(model.ItemsPerPage, totalRecord),
                TotalItems = totalRecord
            };
            return result;
        }

        public async Task<Response> UpdateDiscount(Guid id, UpdateDiscountModel model)
        {
            var entity = await _unitOfWork.DiscountRepository.GetById(id);
            if (entity == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy mã giảm giá này!"
                };
            }
            DateTime timeEnd = model.TimeEnd != null ? model.TimeEnd.Value : entity.TimeEnd;
            if (model.TimeStart != null && DateTime.Compare(model.TimeStart.Value, timeEnd) > 0)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = $"Thời gian bắt đầu không được lớn hơn thời gian kết thúc({timeEnd})"
                };
            }
            DateTime timeStart = model.TimeStart != null ? model.TimeStart.Value : entity.TimeStart;
            if (model.TimeEnd != null && DateTime.Compare(model.TimeEnd.Value, timeStart) < 0)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = $"Thời gian kết thúc không được nhỏ hơn thời gian bắt đầu({timeStart})"
                };
            }

            entity.PhotoUrl = await DeleteFile(model.DeleteFile, Container.Admin, entity.PhotoUrl);
            entity.PhotoUrl += await UploadFile(model.UploadFile, Container.Admin);
            entity.ServiceTypeId = UpdateTypeOfNotNullAbleObject<Guid>(entity.ServiceTypeId, model.ServiceTypeId);
            entity.Title = UpdateTypeOfNullAbleObject<string>(entity.Title, model.Title);
            entity.Description = UpdateTypeOfNullAbleObject<string>(entity.Description, model.Description);
            entity.TimeStart = UpdateTypeOfNotNullAbleObject<DateTime>(entity.TimeStart, model.TimeStart);
            entity.TimeEnd = UpdateTypeOfNotNullAbleObject<DateTime>(entity.TimeEnd, model.TimeEnd);
            entity.Value = UpdateTypeOfNotNullAbleObject<decimal>(entity.Value, model.Value);
            entity.Status = UpdateTypeOfNotNullAbleObject<int>(entity.Status, model.Status);

            _unitOfWork.DiscountRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return new()
            {
                StatusCode = 201,
                Message = "Cập nhật mã giảm giá thành công!"
            };
        }

        private async Task<Response> SendDiscountToCustomer(Guid customerId, Guid discountId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy khách hàng!"
                };
            }

            var discount = await _unitOfWork.DiscountRepository.GetById(discountId);
            if (discount == null)
            {
                return new()
                {
                    StatusCode = 404,
                    Message = "Mã giảm giá không hợp lệ!"
                };
            }

            switch (discount.Status)
            {
                case (int)DiscountStatus.Disabled:
                    return new() { StatusCode = 400, Message = "Mã giảm giá đã bị vô hiệu hóa!" };

                case (int)DiscountStatus.BeSent:
                    return new() { StatusCode = 400, Message = "Mã giảm giá đã được gửi cho một khách hàng khác!" };

                case (int)DiscountStatus.BeUsed:
                    return new() { StatusCode = 400, Message = "Mã giảm giá đã được sử dụng!" };

                case (int)DiscountStatus.Expire:
                    return new() { StatusCode = 400, Message = "Mã giảm giá đã hết hạn sử dụng!" };

                default: break;
            }

            if (discount.Status == (int)DiscountStatus.UnSent)
            {
                SaveNotificationModel notiModel = new SaveNotificationModel()
                {
                    CustomerId = customer.CustomerId.ToString(),
                    CustomerFirstName = customer.FirstName,
                    CustomerLastName = customer.LastName,
                    Title = "Mã giảm giá từ hệ thông",
                    Type = "Discount",
                    Message = $"Bạn vừa nhận được mã giảm giá {discount.Code}. Hãy áp dụng khi thanh toán hóa đơn."
                };
                await _notiService.SaveNotification(notiModel);
                return new()
                {
                    StatusCode = 201,
                    Message = "Gửi mã giảm giá cho khách hàng thành công!"
                };
            }

            return new()
            {
                StatusCode = 500,
                Message = "Lỗi từ hệ thống khi gửi mã giảm giá cho khách hàng"
            };
        }

        public async Task<Response> CheckAvaliableDiscount(string discountCode)
        {
            var discount = await _unitOfWork.DiscountRepository
                            .Query()
                            .Where(x => x.Code == discountCode)
                            .FirstOrDefaultAsync();

            if (discount == null)
            {
                return new()
                {
                    StatusCode = 400,
                    Message = "Mã giảm giá không đúng!"
                };
            }

            switch (discount.Status)
            {
                case (int)DiscountStatus.Disabled:
                    return new() { StatusCode = 400, Message = "Mã giảm giá này đã bị vô hiệu hóa!" };

                case (int)DiscountStatus.BeUsed:
                    return new() { StatusCode = 400, Message = "Mã giảm giá này đã được sử dụng!" };

                case (int)DiscountStatus.Expire:
                    return new() { StatusCode = 400, Message = "Mã giảm giá đã hết hạn sử dụng!" };

                default: break;
            }

            return new()
            {
                StatusCode = 200,
                Data = discount,
                Message = "Mã giảm giá hợp lệ!"
            };
        }

        public async Task<List<Discount>> GetDiscountsWithStatusCondition()
        {
            return await _unitOfWork.DiscountRepository
                    .Query()
                    .Where(x => x.Status != (int)DiscountStatus.Disabled && x.Status != (int)DiscountStatus.Expire)
                    .ToListAsync();
        }

        public async Task UpdateDiscountStatus(Discount discount)
        {
            _unitOfWork.DiscountRepository.Update(discount);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Response> SendDiscountToCustomers(SendDiscountToCustomer model)
        {
            foreach(Guid x in model.CustomerIdList)
            {
                var result= await SendDiscountToCustomer(x, model.DiscountId);
                if(result.StatusCode!= 201)
                {
                    return result;
                }
            }
            return new()
            {
                StatusCode = 201,
                Message = "Gửi mã giảm giá cho khách hàng thành công!"
            };
        }
    }
}