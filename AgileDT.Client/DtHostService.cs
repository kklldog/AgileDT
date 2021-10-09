using AgileDT.Client.Classes;
using AgileDT.Client.Sgr;
using Microsoft.CodeAnalysis.Host;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgileDT.Client
{
    public class DtHostedService : IHostedService
    {
        ILogger<DtHostedService> _logger;
        IServiceProvider _serviceProvider;
        public DtHostedService(ILogger<DtHostedService> logger, IServiceProvider serviceProvider){
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            SgrClient.Instance.AddMessageHandler(new QueryStatusMessageHandler(_serviceProvider));
            SgrClient.Instance.ClientConnected += async ()=>{
                _logger.LogInformation("signalR client connect to hub successful .");
                var types = ServiceProxyManager.Instance.GetSourceTypes();
                foreach (var type in types)
                {
                    await SgrClient.Instance.SendMessageToHub("RegisterEvent", type.GetEventName());
                }
            };
            try
            {
                await SgrClient.Instance.ConnectAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Try to connect signalR hub err . ");
                throw;
            }
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
