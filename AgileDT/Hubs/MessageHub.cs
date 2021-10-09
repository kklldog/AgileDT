using AgileDT.Data;
using AgileDT.Data.Entities;
using AgileDT.MessageQueue;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileDT.Hubs
{
    public class MessageHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("MessageHub add a client .");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("MessageHub remove a client .");

            ClientCollection.Remove(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task ReturnQueryStatusResult(string message)
        {
            Console.WriteLine($"Hub receive message type:ReturnQueryStatusResult message:{message}");

            dynamic dy = JsonConvert.DeserializeObject<dynamic>(message);
            string id = dy.id;
            int status = dy.status;

            var eventMsg = FreeSQL.Instance.Select<EventMessage>().Where(x => x.EventId == id).ToOne();
            if (eventMsg != null)
            {
                await FreeSQL.Instance.Update<EventMessage>()
                    .Set(x => x.Status, (MessageStatus)status)
                    .Where(x => x.EventId == id)
                    .ExecuteAffrowsAsync();
            }

        }

        public Task RegisterEvent(string eventName)
        {
            Console.WriteLine($"Hub receive message type:RegisterEvent message:{eventName}");
            ClientCollection.Add(eventName, Context.ConnectionId);
            MQ.BindQueue(eventName);

            return Task.CompletedTask;
        }
    }
}
