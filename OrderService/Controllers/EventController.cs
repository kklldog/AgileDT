using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderService.Data;
using OrderService.Data.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;

        public EventController(ILogger<EventController> logger)
        {
            _logger = logger;
        }

        [HttpGet("query")]
        public async Task<object> Query(string id)
        {
            var eventMsg = await FreeSQL.Instance.Select<EventMessage>().Where(x => x.EventId == id).FirstAsync();

            if (eventMsg == null || eventMsg.Status == MessageStatus.Cancel)
            {
                return new
                {
                    success = true,
                    status = MessageStatus.Cancel
                };
            }

            if (eventMsg.Status == MessageStatus.Done)
            {
                return new
                {
                    success = true,
                    status = MessageStatus.Done
                };
            }

            if (eventMsg.Status == MessageStatus.Prepare)
            {
                if ((DateTime.Now - eventMsg.CreateTime.Value).TotalSeconds > 60)
                {
                    //如果 prepare 状态大于60s，直接认为已经取消
                    return new
                    {
                        success = true,
                        status = MessageStatus.Cancel
                    };
                }

                var order = await FreeSQL.Instance.Select<Order>().Where(x => x.EventId == id).FirstAsync();
                if (order == null)
                {
                    return new
                    {
                        success = true,
                        status = MessageStatus.Cancel
                    };
                }
            }

            return new
            {
                success = true,
                status = MessageStatus.Prepare
            };
        }
    }
}
