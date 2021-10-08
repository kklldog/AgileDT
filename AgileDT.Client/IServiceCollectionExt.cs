using AgileDT.Client;
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

            var eventNames = new List<string>();

            var classCreator = new ClassProxyCreator();
            var ass = classCreator.CreateProxyAssembly();

            var ieventT = typeof(IEventService);
            var types = ass.GetTypes();
            foreach (var type in types)
            {
                var ites = type.GetInterfaces();
                var it = ites.FirstOrDefault(x => ieventT.IsAssignableFrom(x));
                if (it == null)
                {
                    continue;
                }

                serviceCollection.AddScoped(it, type);
                eventNames.Add(type.Name.TrimEnd("_agiledt_proxy".ToArray()));
            }

            serviceCollection.AddHostedService<DtHostedService>();
        }
    }
}
