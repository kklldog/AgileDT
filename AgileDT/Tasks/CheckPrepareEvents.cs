using AgileDT.Data;
using AgileDT.Data.Entities;
using AgileDT.Hubs;
using AgileHttp;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AgileDT.Tasks
{
    public class CheckPrepareEvents : IHostedService
    {
        class QueryResult
        {
            public bool success { get; set; }

            public MessageStatus status { get; set; }
        }

        private readonly ILogger<CheckPrepareEvents> _logger;
        private readonly IHubContext<MessageHub> _hub;

        public CheckPrepareEvents(ILogger<CheckPrepareEvents> logger, IHubContext<MessageHub> hub)
        {
            _logger = logger;
            _hub = hub;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            new Thread(() =>
            {
                _logger.LogInformation("CheckPrepareEvents task started .");

                while (true)
                {
                    try
                    {
                        Check();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "CheckPrepareEvents action fail .");
                    }

                    Thread.Sleep(1000 * 10);
                }

            }).Start();

            return Task.CompletedTask;
        }

        private void Check()
        {
            _logger.LogInformation($"start to check  prepare events.");

            var dt = DateTime.Now.AddSeconds(-10);
            var events = FreeSQL.Instance
                .Select<EventMessage>()
                // 获取10s之前的prepare的任务
                .Where(x => x.Status == MessageStatus.Prepare && x.CreateTime < dt)
                .ToList();
            foreach (var et in events)
            {
                _logger.LogInformation($"start to check event {et.EventId} status .");
                try
                {
                    var client = ClientCollection.GetGroupClients(et.EventName).FirstOrDefault();
                    if (client != null)
                    {
                        _hub.Clients.Client(client).SendAsync("QueryStatus", JsonConvert.SerializeObject(new
                        {
                            id = et.EventId,
                            eventName = et.EventName
                        }));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"check event {et.EventId} prepare status fail , id {et.EventId}");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("CheckPrepareEvents task stoped .");

            return Task.CompletedTask;
        }
    }
}
