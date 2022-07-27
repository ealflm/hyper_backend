using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TourismSmartTransportation.Business.Interfaces.Partner;
using TourismSmartTransportation.Business.Interfaces.Shared;
using TourismSmartTransportation.Business.SearchModel.Partner.Route;
using TourismSmartTransportation.Business.SearchModel.Partner.VehicelManagement;

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

                            TripSearchModel tripSearchModel = new TripSearchModel()
                            {
                                VehicleId = Guid.Parse(vehicleTracking.VehicleId)
                            };
                            var vehicleTripsList = (await vehicleTripScopeService.GetTripsList(tripSearchModel)).Items;

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
    }
}