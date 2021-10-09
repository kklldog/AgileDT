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
        public static void AddAgileDT(this IServiceCollection serviceCollection, IConfiguration config)
        {
            Config.Instance = config;
            ServiceProxyManager.Instance.ScanAndBuild();
            var proxyMap = ServiceProxyManager.Instance.GetInterfaceProxyMap();

            foreach (var item in proxyMap)
            {
                serviceCollection.AddScoped(item.Value, item.Key);
            }

            serviceCollection.AddHostedService<DtHostedService>();
        }
    }
}
