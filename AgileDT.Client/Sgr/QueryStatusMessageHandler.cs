using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgileDT.Client.Sgr
{
    public class QueryStatusMessageHandler : IMessageHandler
    {
        public string Type => "QueryStatus";
        public void Handle(string message)
        {
            Console.WriteLine("QueryStatusMessageHandler handle message " + message);

            dynamic dy = JsonConvert.DeserializeObject<dynamic>(message);

            string eventName = dy.eventName;
            string id = dy.id;

            var status = new DefaultEventService().QueryEventStatus(id);

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
