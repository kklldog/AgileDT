using AgileDT.Client.Classes;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgileDT.Client.Sgr
{
    public class QueryStatusMessageHandler : IMessageHandler
    {
        IServiceProvider _serviceProvider;

        public QueryStatusMessageHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Type => "QueryStatus";
        public void Handle(string message)
        {
            Console.WriteLine("QueryStatusMessageHandler handle message " + message);

            dynamic dy = JsonConvert.DeserializeObject<dynamic>(message);

            string eventName = dy.eventName;
            string id = dy.id;


            IEventService service = null;
            var sourceType = ServiceProxyManager.Instance.FindSourceTypeByEventName(eventName);
            if (sourceType == null)
            {
                service = new DefaultEventService();
            }
            else
            {
                var itf = ServiceProxyManager.Instance.FindInterfaceBySourceType(sourceType);
                using var sc = _serviceProvider.CreateScope();
                var obj = sc.ServiceProvider.GetRequiredService(itf);

                service = obj as IEventService;
            }

            var status = service.QueryEventStatus(id);

            Console.WriteLine($"QueryEventStatus id:{id} status:{status} ");

            var msg = JsonConvert.SerializeObject(new
            {
                id,
                status
            });
            SgrClient.Instance.SendMessageToHub("ReturnQueryStatusResult", msg);

            Console.WriteLine($"SendMessageToHub method ReturnQueryStatusResult , Message: {msg}");
        }
    }
}
