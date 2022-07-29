using System;
using System.Collections.Generic;
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

namespace TourismSmartTransportation.API
{
    public class HyperBackgroundService : BackgroundService
    {
        private readonly ILogger<HyperBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceProvider;

        public HyperBackgroundService(ILogger<HyperBackgroundService> logger, IServiceScopeFactory serviceProvider)
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
                    _logger.LogInformation("From HyperBackgroundService: ExecuteAsync");

                    // Inject service
                    var vehicleScopeService = scope.ServiceProvider.GetRequiredService<IVehicleManagementService>();
                    var vehicleTrackingScopeService = scope.ServiceProvider.GetRequiredService<IVehicleCollectionService>();
                    var vehicleTripScopeService = scope.ServiceProvider.GetRequiredService<ITripManagementService>();
                    var firebaseCloudMsgScopeService = scope.ServiceProvider.GetRequiredService<IFirebaseCloudMsgService>();
                    var customerScopeService = scope.ServiceProvider.GetRequiredService<ICustomerManagementService>();
                    var customerTripScopeService = scope.ServiceProvider.GetRequiredService<ICustomerTripService>();

                    // Processing
                    VehicleProcess(vehicleScopeService, vehicleTrackingScopeService, vehicleTripScopeService);
                    // RentingServiceNotificationProcess(firebaseCloudMsgScopeService, customerScopeService, customerTripScopeService);

                    // Interval in specific time
                    await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
                }

            }
        }

        public static string UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime.ToString("HH:mm");
        }

        public async static void VehicleProcess(IVehicleManagementService vehicleScopeService, IVehicleCollectionService vehicleTrackingScopeService,
                                            ITripManagementService vehicleTripScopeService)
        {
            // Get all vehicle from database
            VehicleSearchModel model = new VehicleSearchModel();
            var vehiclesList = await vehicleScopeService.Search(model);

            // Check time to update status of vehicle
            foreach (var vehicle in vehiclesList)
            {
                var vehicleTracking = await vehicleTrackingScopeService.GetByVehicleId(vehicle.Id.ToString());
                if (vehicleTracking.Id != "-1")
                {
                    // format time
                    var timeFormat = UnixTimeStampToDateTime(vehicleTracking.CreatedDate);

                    TripSearchModel tripSearchModel = new TripSearchModel() // create model to call service
                    {
                        VehicleId = Guid.Parse(vehicleTracking.VehicleId) // parse to GUID type
                    };
                    var vehicleTripsList = (await vehicleTripScopeService.GetTripsList(tripSearchModel)).Items; // get list by vehicle

                    bool flag = false;
                    for (int i = 0; i < vehicleTripsList.Count; i++)
                    {
                        if (vehicleTripsList[i].TimeStart.CompareTo(timeFormat) <= 0 && vehicleTripsList[i].TimeEnd.CompareTo(timeFormat) >= 0)
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (flag)
                    {
                        if (vehicle.Status != 4)
                        {
                            UpdateVehicleModel updateVehicleModel = new UpdateVehicleModel()
                            {
                                LicensePlates = vehicle.LicensePlates,
                                Name = vehicle.Name,
                                Color = vehicle.Color,
                                Status = 4
                            };
                            await vehicleScopeService.Update(vehicle.Id, updateVehicleModel);
                        }
                    }
                    else
                    {
                        if (vehicle.Status == 4)
                        {
                            UpdateVehicleModel updateVehicleModel = new UpdateVehicleModel()
                            {
                                LicensePlates = vehicle.LicensePlates,
                                Name = vehicle.Name,
                                Color = vehicle.Color,
                                Status = 1
                            };
                            await vehicleScopeService.Update(vehicle.Id, updateVehicleModel);
                        }
                    }

                }
            }
        }

        public async static void RentingServiceNotificationProcess(IFirebaseCloudMsgService firebaseService,
                                                                    ICustomerManagementService customerScopeService,
                                                                    ICustomerTripService customerTripScopeService)
        {
            string title = "Dịch vụ thuê xe Hyper thông báo";
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
                                DateTime.Now.CompareTo(approval30MinsTimeOver.AddMinutes(1)) <= 0 &&
                                DateTime.Now.CompareTo(approval30MinsTimeOver.Subtract(TimeSpan.FromMinutes(1))) >= 0
                            )
                            ||
                            (
                                DateTime.Now.CompareTo(approval15MinsTimeOver.AddMinutes(1)) <= 0 &&
                                DateTime.Now.CompareTo(approval15MinsTimeOver.Subtract(TimeSpan.FromMinutes(1))) >= 0
                            )
                            ||
                            (
                                DateTime.Now.CompareTo(approval5MinsTimeOver.AddMinutes(1)) <= 0 &&
                                DateTime.Now.CompareTo(approval5MinsTimeOver.Subtract(TimeSpan.FromMinutes(1))) >= 0
                            )
                        )
                    )
                {
                    var customer = await customerScopeService.GetCustomer(customerTripsList[i].CustomerId);
                    if (!string.IsNullOrEmpty(customer.RegistrationToken))
                    {
                        string message = $"Thời gian thuê xe của quý khách sẽ hết hạn lúc {customerTripsList[i].RentDeadline}. Quý khách vui lòng trả xe đúng giờ để không bị phát sinh chi phí!";
                        await firebaseService.SendNotificationForRentingService(customer.RegistrationToken, title, message);
                    }
                }

                if (DateTime.Now.CompareTo(customerTripsList[i].RentDeadline.Value.AddMinutes(10)) >= 0)
                {
                    var customer = await customerScopeService.GetCustomer(customerTripsList[i].CustomerId);
                    customerTripSearchModel.Status = 2;
                    var result = await customerTripScopeService.UpdateStatusCustomerTrip(customerTripsList[i].CustomerTripId, customerTripSearchModel);
                    if (result.StatusCode == 201 && !string.IsNullOrEmpty(customer.RegistrationToken))
                    {
                        string message = $"Thời gian thuê xe của quý khác đã quá hạn.";
                        await firebaseService.SendNotificationForRentingService(customer.RegistrationToken, title, message);
                    }
                }
            }
        }
    }
}