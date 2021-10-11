using AgileDT.Client.Classes;
using AgileDT.Client.Data;
using AgileDT.Client.MessageQueue;
using AgileHttp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgileDT.Client.Consumer
{
    internal class ConsumerRegister
    {
        class ApiResult
        {
            public bool success { get; set; }
        }

        private IServiceProvider _serviceProvider;
        private ILogger _logger;
        public ConsumerRegister(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var lf = serviceProvider.GetService<ILoggerFactory>();
            _logger = lf.CreateLogger<ConsumerRegister>();
        }
        public void Register()
        {
            var map = GetInterfaceTypeMap();
            foreach (var item in map)
            {
                var eventName = item.Key.GetEventName();
                var itf = item.Value;

                MQ.BindConsumer(eventName, (m, args) =>
                {
                    try
                    {
                        var message = Encoding.UTF8.GetString(args.Body.ToArray());
                        Console.WriteLine($"receive message ： {message}");
                        var eventMsg = JsonConvert.DeserializeObject<EventMessage>(message);

                        using var sc = _serviceProvider.CreateScope();
                        var obj = sc.ServiceProvider.GetRequiredService(itf);
                        var imp = obj as IEventMessageHandler;
                        var service = imp;
                        var ret = service.Receive(eventMsg);
                        if (ret)
                        {
                            string eventId = eventMsg.EventId;
                            var url = ServerBaseUrl + "/api/Message";
                            using var resp = url.AsHttpClient()
                            .Config(new RequestOptions
                            {
                                ContentType = "application/json"
                            })
                            .Post(new
                            {
                                eventId = eventId,
                                status = 4
                            });
                            var result = resp.Deserialize<ApiResult>();
                            if (result.success)
                            {
                                m.BasicAck(args.DeliveryTag, false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "when consume message occur error .");
                    }
                });
            }
        }

        /// <summary>
        /// 获取IEventMessageHandler相关接口与实现的对应关系，key为type，value为interface
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Type,Type> GetInterfaceTypeMap()
        {
            var dict = new Dictionary<Type, Type>();
            var types = Helper.ScanAllMessageHandlers();
            var ieventT = typeof(IEventMessageHandler);
            foreach (var type in types)
            {
                var ites = type.GetInterfaces();
                var it = ites.FirstOrDefault(x => ieventT.IsAssignableFrom(x));
                if (it == null)
                {
                    continue;
                }

                dict.Add(type, it);
            }

            return dict;
        }

        private string ServerBaseUrl
        {
            get
            {
                var agiledtServer = Config.Instance["agiledt:server"];
                agiledtServer = agiledtServer.TrimEnd('/');

                return agiledtServer;
            }
        }
    }
}
