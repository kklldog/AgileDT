using AgileDT.Data;
using AgileDT.Data.Entities;
using AgileDT.MessageQueue;
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
    public class MessageReSend : IHostedService
    {
        private readonly ILogger<MessageReSend> _logger;

        public MessageReSend(ILogger<MessageReSend> logger)
        {
            _logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            new Thread(() =>
            {
                _logger.LogInformation("MessageReSend task started .");

                while (true)
                {
                    try
                    {
                        TryResend();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "MessageReSent action fail .");
                    }

                    Thread.Sleep(1000 * 30);
                }

            }).Start();

            return Task.CompletedTask;
        }

        private void TryResend()
        {
            var dt = DateTime.Now.AddSeconds(-30);
            var events = FreeSQL.Instance
                .Select<EventMessage>()
                // 获取30s后还处在待发送跟已发送未确认的消息
                .Where(x => (x.Status == MessageStatus.WaitSend || x.Status == MessageStatus.Sent) && x.CreateTime < dt)
                .ToList();
            foreach (var et in events)
            {
                _logger.LogInformation($"start to resend event {et.EventId} to mq .");
                try
                {
                    MQ.Push(et, et.EventName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"resend event {et.EventId} to mq fail .");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MessageReSend task stoped .");

            return Task.CompletedTask;
        }
    }
}
