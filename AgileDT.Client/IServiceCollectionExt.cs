using AgileDT.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AgileDT.Client
{
    public static class ServiceCollectionExt
    {
        public static void AddAgileDT(this IServiceCollection serviceCollection, IConfiguration config)
        {
            Config.Instance = config;

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
            }
        }
    }
}
