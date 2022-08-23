using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using MongoDB.Driver;
using TourismSmartTransportation.Business.CommonModel;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.SearchModel.Shared.NotificationCollection;
using TourismSmartTransportation.Data.Interfaces;
using TourismSmartTransportation.Data.MongoCollections.Notification;
using TourismSmartTransportation.Data.MongoDBContext;

namespace TourismSmartTransportation.Business.Implements.Shared
{
    public class NotificationCollectionService : BaseService, INotificationCollectionService
    {
        private readonly IMongoCollection<NotificationCollection> _notificationService;

        public NotificationCollectionService(IUnitOfWork unitOfWork, BlobServiceClient blobServiceClient,
                                                MongoDBContext mongo, IMongoCosmosDBSettings settings) : base(unitOfWork, blobServiceClient)
        {
            _notificationService = mongo.GetNotificationsCollection;
        }

        public async Task<Response> DisableNotificationStatus(string customerId, string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                var notisList1 = await _notificationService.Find(x => x.CustomerId == customerId).ToListAsync();
                foreach (var notiItem in notisList1)
                {
                    if (notiItem.Status == (int)NotificationStatus.BeNotUsed)
                    {
                        continue;
                    }

                    var filter = Builders<NotificationCollection>.Filter.Eq("_id", notiItem.Id);
                    var update = Builders<NotificationCollection>.Update.Set("Status", (int)NotificationStatus.Disabled);
                    _notificationService.UpdateOne(filter, update);
                }

                return new()
                {
                    StatusCode = 201,
                    Message = "Thông báo đã bị vô hiệu hóa"
                };
            }

            var notisList2 = await _notificationService.Find(x => x.CustomerId == customerId && x.Type == type).ToListAsync();
            foreach (var notiItem in notisList2)
            {
                if (notiItem.Status == (int)NotificationStatus.BeNotUsed)
                {
                    continue;
                }

                var filter = Builders<NotificationCollection>.Filter.Eq("_id", notiItem.Id);
                var update = Builders<NotificationCollection>.Update.Set("Status", (int)NotificationStatus.Disabled);
                _notificationService.UpdateOne(filter, update);
            }

            return new()
            {
                StatusCode = 201,
                Message = "Thông báo đã bị vô hiệu hóa"
            };
        }

        public async Task<List<NotificationCollection>> GetNotificationsByCustomer(string customerId, string type, bool isGetAll = false)
        {
            return await _notificationService.Find(GetNotificationWithCondition(customerId, type, isGetAll))
                    .SortByDescending(x => x.Id)
                    .ToListAsync();
        }

        public async Task<Response> SaveNotification(SaveNotificationModel noti)
        {
            NotificationCollection model = new NotificationCollection()
            {
                CustomerId = noti.CustomerId,
                CustomerName = noti.CustomerFirstName + " " + noti.CustomerLastName,
                Title = noti.Title,
                Message = noti.Message,
                Type = noti.Type,
                CreatedDateTimeStamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                ModifiedDateTimeStamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
            };

            await _notificationService.InsertOneAsync(model);

            return new()
            {
                StatusCode = 200,
                Message = "Tạo mới thành công!"
            };
        }

        private Expression<Func<NotificationCollection, bool>> GetNotificationWithCondition(string customerId, string type, bool isGetAll = false)
        {
            if (isGetAll && string.IsNullOrEmpty(type)) // lấy tất cả thông báo
                return x => x.CustomerId == customerId;

            if (isGetAll) // lấy tất cả thông báo với type cụ thể
                return x => x.CustomerId == customerId && x.Type == type;

            if (string.IsNullOrEmpty(type)) // chỉ lấy những thông báo active
                return x => x.CustomerId == customerId && x.Status == 1;

            return x => x.CustomerId == customerId && x.Type == type && x.Status == 1; // chỉ lấy những thông báo active và với type cụ thể
        }
    }
}