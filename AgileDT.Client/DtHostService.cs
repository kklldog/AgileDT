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
        public DtHostedService(ILogger<DtHostedService> logger){
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await SgrClient.Instance.ConnectAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Try to connect signalR hub err .");
            }
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    if (SgrClient.Instance.Connected)
                    {
                        _logger.LogInformation("signalR client connect to hub successful .");
                        SgrClient.Instance.AddMessageHandler(new QueryStatusMessageHandler());
                        var types = Helper.ScanAll();
                        foreach (var type in types)
                        {
                            await SgrClient.Instance.SendMessageToHub("RegisterEvent", type.Name);
                        }
                        break;
                    }
                    await Task.Delay(1000);
                }
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
