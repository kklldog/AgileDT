using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileDT.Hubs
{
    public class MessageHub : Hub
    {
        public Task ReturnQueryStatusResult(int status)
        {
            Console.WriteLine($"hub receive message type:ReturnQueryStatusResult message:{status}");

            return Task.CompletedTask;
        }

        public Task RegisterEvent(string eventName)
        {
            Console.WriteLine($"hub receive message type:RegisterEvent message:{eventName}");

            return Task.CompletedTask;
        }
    }
}
