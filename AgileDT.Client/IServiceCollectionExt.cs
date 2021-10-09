using AgileDT.Client;
using AgileDT.Client.Classes;
using AgileDT.Client.Sgr;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgileDT.Client
{
    public static class ServiceCollectionExt
    {
        public static void AddAgileDT(this IServiceCollection serviceCollection)
        {
            ServiceProxyManager.Instance.ScanAndBuild();
            var proxyMap = ServiceProxyManager.Instance.GetInterfaceProxyMap();
            foreach (var item in proxyMap)
            {
                //注册接口跟事件代理类到ioc容器
                serviceCollection.AddScoped(item.Value, item.Key);
                Console.WriteLine($"add scoped {item.Value} => {item.Key}");
            }

            var messageHandlerMap = Consumer.ConsumerRegister.GetInterfaceTypeMap();
            foreach (var item in messageHandlerMap)
            {
                //注册接口跟messageHandler的实现类到ioc容器
                serviceCollection.AddScoped(item.Value, item.Key);
                Console.WriteLine($"add scoped {item.Value} => {item.Key}");
            }
            //注册启动服务
            serviceCollection.AddHostedService<DtHostedService>();
        }
    }
}
