using AgileDT.Client.Classes;
using AgileDT.Client.Consumer;
using AgileDT.Client.Sgr;
using Microsoft.CodeAnalysis.Host;
using Microsoft.Extensions.Configuration;
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
        public DtHostedService(ILogger<DtHostedService> logger, IServiceProvider serviceProvider, IConfiguration config){
            Config.Instance = config;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //注册主动方状态的Handler
            SgrClient.Instance.AddMessageHandler(new QueryStatusMessageHandler(_serviceProvider));
            //长连接建立后注册主动方会发起的事件到AgileDt Server
            SgrClient.Instance.ClientConnected += async ()=>{
                _logger.LogInformation("signalR client connect to hub successful .");
                var types = ServiceProxyManager.Instance.GetSourceTypes();
                foreach (var type in types)
                {
                    await SgrClient.Instance.SendMessageToHub("RegisterEvent", type.GetEventName());
                }
            };
            //异步开始连接 signalr hub ，为了不阻塞程序启动
            _ = Task.Run(async ()=> {
                while (true)
                {
                    try
                    {
                        await SgrClient.Instance.ConnectAsync();
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Try to connect signalR hub err . ");
                    }
                    await Task.Delay(1000);
                }
            });
            //注册消息的消费服务，绑定MQ对应的队列
            new ConsumerRegister(_serviceProvider).Register();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
