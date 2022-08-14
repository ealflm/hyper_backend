using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TourismSmartTransportation.Business.Interfaces.Admin;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.SearchModel.Partner.Route;
using TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement;
using TourismSmartTransportation.Business.SearchModel.Shared;
using TourismSmartTransportation.Business.SearchModel.Shared.NotificationCollection;

namespace TourismSmartTransportation.API
{
    public class HyperBackgroundService1 : BackgroundService
    {
        private readonly ILogger<HyperBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceProvider;

        public HyperBackgroundService1(ILogger<HyperBackgroundService> logger, IServiceScopeFactory serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    _logger.LogInformation("Start HyperBackgroundService1: ExecuteAsync");

                    // Inject service
                    var firebaseCloudMsgScopeService = scope.ServiceProvider.GetRequiredService<IFirebaseCloudMsgService>();
                    var customerScopeService = scope.ServiceProvider.GetRequiredService<ICustomerManagementService>();
                    var customerTripScopeService = scope.ServiceProvider.GetRequiredService<ICustomerTripService>();
                    var notificationScopeService = scope.ServiceProvider.GetRequiredService<INotificationCollectionService>();

                    // Processing
                    await RentingServiceNotificationProcess(firebaseCloudMsgScopeService, customerScopeService, customerTripScopeService, notificationScopeService);

                    // Interval in specific time
                    await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
                }

            }
            _logger.LogInformation("End HyperBackgroundService1: ExecuteAsync");
        }

        public async Task RentingServiceNotificationProcess(IFirebaseCloudMsgService firebaseService,
                                                                    ICustomerManagementService customerScopeService,
                                                                    ICustomerTripService customerTripScopeService,
                                                                    INotificationCollectionService notificationScopeService)
        {
            _logger.LogInformation("Start RentingServiceNotificationProcess");
            string title = "Dịch vụ thuê xe";
            var customerTripSearchModel = new CustomerTripSearchModel();
            var customerTripsList = await customerTripScopeService.GetCustomerTripsListForRentingService(customerTripSearchModel);
            for (int i = 0; i < customerTripsList.Count; i++)
            {
                DateTime approval30MinsTimeOver = customerTripsList[i].RentDeadline.Value.Subtract(TimeSpan.FromMinutes(30));
                DateTime approval15MinsTimeOver = customerTripsList[i].RentDeadline.Value.Subtract(TimeSpan.FromMinutes(15));
                DateTime approval5MinsTimeOver = customerTripsList[i].RentDeadline.Value.Subtract(TimeSpan.FromMinutes(5));


                // Thông báo tới khách thời gian thuê xe sắp hết trước 30p, 15p và 5p
                if (
                        (
                            (
                                DateTime.UtcNow.CompareTo(approval30MinsTimeOver.AddSeconds(35)) <= 0 &&
                                DateTime.UtcNow.CompareTo(approval30MinsTimeOver.Subtract(TimeSpan.FromSeconds(35))) >= 0
                            )
                            ||
                            (
                                DateTime.UtcNow.CompareTo(approval15MinsTimeOver.AddSeconds(35)) <= 0 &&
                                DateTime.UtcNow.CompareTo(approval15MinsTimeOver.Subtract(TimeSpan.FromSeconds(35))) >= 0
                            )
                            ||
                            (
                                DateTime.UtcNow.CompareTo(approval5MinsTimeOver.AddSeconds(1)) <= 0 &&
                                DateTime.UtcNow.CompareTo(approval5MinsTimeOver.Subtract(TimeSpan.FromSeconds(35))) >= 0
                            )
                        )
                    )
                {
                    var customer = await customerScopeService.GetCustomer(customerTripsList[i].CustomerId);
                    if (!string.IsNullOrEmpty(customer.RegistrationToken))
                    {
                        string message = $"Thời gian thuê xe của quý khách sẽ hết hạn lúc {customerTripsList[i].RentDeadline?.ToString("HH:mm - dd/MM/yyyy")}. Quý khách vui lòng trả xe đúng giờ để không bị phát sinh chi phí!";
                        await firebaseService.SendNotificationForRentingService(customer.RegistrationToken, title, message);
                        SaveNotificationModel noti = new SaveNotificationModel()
                        {
                            CustomerId = customer.Id.ToString(),
                            CustomerFirstName = customer.FirstName,
                            CustomerLastName = customer.LastName,
                            Title = title,
                            Message = message,
                            Type = "Thông báo"
                        };
                        await notificationScopeService.SaveNotification(noti);
                    }
                }

                if (DateTime.UtcNow.CompareTo(customerTripsList[i].RentDeadline.Value.AddMinutes(10)) >= 0) // out of limit time than 10 minutes
                {
                    var customer = await customerScopeService.GetCustomer(customerTripsList[i].CustomerId);
                    customerTripSearchModel.Status = (int)CustomerTripStatus.Overdue;
                    var result = await customerTripScopeService.UpdateStatusCustomerTrip(customerTripsList[i].CustomerTripId, customerTripSearchModel);
                    if (result.StatusCode == 201 && !string.IsNullOrEmpty(customer.RegistrationToken))
                    {
                        string message = $"Thời gian thuê xe của quý khác đã quá hạn 10 phút.";
                        await firebaseService.SendNotificationForRentingService(customer.RegistrationToken, title, message);
                    }
                }
            }
            _logger.LogInformation("End RentingServiceNotificationProcess");
        }
    }
}