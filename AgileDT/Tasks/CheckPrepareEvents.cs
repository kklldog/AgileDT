using AgileDT.Data;
using AgileDT.Data.Entites;
using AgileHttp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

        public CheckPrepareEvents(ILogger<CheckPrepareEvents> logger)
        {
            _logger = logger;
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
            var events = FreeSQL.Instance
                .Select<EventMessage>()
                // 获取10s之前的prepare的任务
                .Where(x => x.Status == MessageStatus.Prepare && x.CreateTime < DateTime.Now.AddSeconds(-10))
                .ToList();
            foreach (var et in events)
            {
                _logger.LogInformation($"start to check event {et.EventId} status .");
                try
                {
                    //var query = et.QueryApi.AppendQueryString("id", et.EventId);
                    //using var resp = query.AsHttp().Send();
                    //dynamic result = resp.Deserialize<QueryResult>();
                    //if (result.success)
                    //{
                    //    var status = result.status;
                    //    _logger.LogInformation($"remote api response event {et.EventId} status is {status} .");

                    //    if (status == MessageStatus.Prepare)
                    //    {
                    //        continue;
                    //    }
                    //    else
                    //    {
                    //        et.Status = status;
                    //        var ret = FreeSQL.Instance.Update<EventMessage>()
                    //            .Set(x => x.Status, et.Status)
                    //            .Where(x => x.EventId == et.EventId && x.Status == MessageStatus.Prepare)
                    //            .ExecuteAffrows();

                    //        if (ret > 0)
                    //        {
                    //            _logger.LogInformation($"Update event {et.EventId} status to {et.Status} .");
                    //        }
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex, $"check event {et.EventId} prepare status fail , url {et.QueryApi.AppendQueryString("id", et.EventId)}");
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
