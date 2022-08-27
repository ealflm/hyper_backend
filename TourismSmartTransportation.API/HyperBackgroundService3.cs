using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TourismSmartTransportation.Business.Hubs;
using TourismSmartTransportation.Business.Hubs.Models;

namespace TourismSmartTransportation.API
{
    public class HyperBackgroundService3 : BackgroundService
    {
        private readonly ILogger<HyperBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceProvider;
        private readonly IHubContext<NotificationHub> _hubcontext;

        public HyperBackgroundService3(ILogger<HyperBackgroundService> logger, IServiceScopeFactory serviceProvider, IHubContext<NotificationHub> hubcontext)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _hubcontext = hubcontext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    try
                    {
                        _logger.LogInformation("Start HyperBackgroundService3: ExecuteAsync");

                        // // Inject service
                        // var notificationHubScopeService = scope.ServiceProvider.GetRequiredService<INotificationHub>();

                        // // Processing
                        // Dictionary<string, Tuple<TransferHubModel, string>> dic = new Dictionary<string, Tuple<TransferHubModel, string>>();
                        // dic = await notificationHubScopeService.AutoCanceledBookingRequestAfterSpecificTime();

                        // foreach (var dicItem in dic)
                        // {
                        //     if (dic.GetValueOrDefault(dicItem.Key) != null)
                        //     {
                        //         await _hubcontext.Clients.Client(dicItem.Value.Item2).SendAsync(dicItem.Value.Item1.Method,
                        //                     new
                        //                     {
                        //                         StatusCode = dicItem.Value.Item1.StatusCode,
                        //                         Type = dicItem.Value.Item1.Type,
                        //                         Message = dicItem.Value.Item1.Message,
                        //                         Driver = dicItem.Value.Item1.Driver,
                        //                         Customer = dicItem.Value.Item1.Customer
                        //                     }
                        //                 );
                        //     }
                        // }

                        // _logger.LogInformation("=================== BACKGROUND SERVICE #3 ============");
                        // _logger.LogInformation("=================== BACKGROUND SERVICE #3 ============");

                        // Interval in specific time
                        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    }
                    catch (System.Exception)
                    {
                        _logger.LogInformation("======= BACKGROUND SERVICE CALL HUB ERROR ========");
                    }
                }

            }
            _logger.LogInformation("End HyperBackgroundService3: ExecuteAsync");
        }
    }
}