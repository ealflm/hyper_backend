using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TourismSmartTransportation.Business.Interfaces.Admin;

namespace TourismSmartTransportation.API
{
    public class HyperBackgroundService2 : BackgroundService
    {
        private readonly ILogger<HyperBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceProvider;

        public HyperBackgroundService2(ILogger<HyperBackgroundService> logger, IServiceScopeFactory serviceProvider)
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
                    _logger.LogInformation("Start HyperBackgroundService2: ExecuteAsync");

                    // Inject service
                    var discountScopeService = scope.ServiceProvider.GetRequiredService<IDiscountService>();

                    // Processing
                    var discountsList = await discountScopeService.GetDiscountsWithStatusCondition();
                    foreach (var discountItem in discountsList)
                    {
                        if (DateTime.UtcNow.CompareTo(discountItem.TimeEnd) > 0)
                        {
                            discountItem.Status = (int)DiscountStatus.Expire;
                            await discountScopeService.UpdateDiscountStatus(discountItem);
                        }
                    }

                    // Interval in specific time
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }

            }
            _logger.LogInformation("End HyperBackgroundService2: ExecuteAsync");
        }
    }
}