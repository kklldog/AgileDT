using AgileDT.MessageQueue;
using Microsoft.AspNetCore.SignalR;
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
            ClientCollection.Add(new AgileDtClient()
            {
                Id = Context.ConnectionId
            });

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ClientCollection.Remove(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public Task ReturnQueryStatusResult(int status)
        {
            Console.WriteLine($"Hub receive message type:ReturnQueryStatusResult message:{status}");

            return Task.CompletedTask;
        }

        public async Task RegisterEvent(string eventName)
        {
            Console.WriteLine($"Hub receive message type:RegisterEvent message:{eventName}");

            var client = ClientCollection.Get(Context.ConnectionId);
            if (client != null)
            {
                client.GroupName = eventName;
                await Groups.AddToGroupAsync(Context.ConnectionId, eventName);
            }
            MQ.BindQueue(eventName);
        }
    }
}
