using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TourismSmartTransportation.Business.Hubs;

namespace TourismSmartTransportation.API
{
    public class HyperBackgroundService3 : BackgroundService
    {
        private readonly ILogger<HyperBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceProvider;

        public HyperBackgroundService3(ILogger<HyperBackgroundService> logger, IServiceScopeFactory serviceProvider)
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
                    _logger.LogInformation("Start HyperBackgroundService3: ExecuteAsync");

                    // Inject service
                    var notificationHubScopeService = scope.ServiceProvider.GetRequiredService<INotificationHub>();

                    // Processing
                    await notificationHubScopeService.AutoCanceledBookingRequestAfterSpecificTime();

                    // Interval in specific time
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }

            }
            _logger.LogInformation("End HyperBackgroundService3: ExecuteAsync");
        }
    }
}