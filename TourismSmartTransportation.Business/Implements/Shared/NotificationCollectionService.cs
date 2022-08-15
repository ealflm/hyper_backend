using System;
using System.Collections.Generic;
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

        public async Task<List<NotificationCollection>> GetNotificationsByCustomer(string customerId, string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return await _notificationService.Find(x => x.CustomerId == customerId)
                                    .SortByDescending(x => x.Id)
                                    .ToListAsync();
            }

            return await _notificationService.Find(x => x.CustomerId == customerId && x.Type == type)
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
    }
}