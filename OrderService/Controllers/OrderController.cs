using AgileHttp;
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
using AgileDT.Client;
using OrderService.Services;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IAddOrderService _addOrderService;

        public OrderController(ILogger<OrderController> logger, IAddOrderService addOrderService)
        {
            _addOrderService = addOrderService;
            _logger = logger;
        }

        [HttpPost]
        public object Add([FromBody] Order order)
        {
            var bizzMsg = new
            {
                orderId = order.Id,
                memberId = order.MemberId,
                point = 20
            };
            var eventMsg = new EventMessage
            {
                EventId = Guid.NewGuid().ToString(),
                Status = MessageStatus.Prepare,
                BizzMsg = JsonConvert.SerializeObject(bizzMsg),
                QueryApi = "http://localhost:5010/api/event/query",
                CreateTime = DateTime.Now
            };
            try
            {
                FreeSQL.Instance.Ado.Transaction(()=> {
                    //1. 往event_message表写 Prepare 数据
                    FreeSQL.Instance.Insert(eventMsg).ExecuteAffrows();

                    //2. 调用可靠消息服务的接口把 prepare 状态传递过去
                    using var resp = "http://localhost:5000/api/message"
                        .AsHttpClient()
                        .Config(new RequestOptions
                        {
                            ContentType = "application/json"
                        })
                        .Post(eventMsg);
                    if (resp.Exception != null)
                    {
                        throw resp.Exception;
                    }
                    if (resp.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        throw new Exception("send Prepare message to agile_dt_server fail .");
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "send Prepare message to agile_dt_server fail .");
                throw;
            }

            //3.dobizz
            try
            {
                _addOrderService.AddOrder(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "add order and update event transaction fail .");
                FreeSQL.Instance.Update<EventMessage>()
                 .Set(x => x.Status, MessageStatus.Cancel)
                 .Where(x => x.EventId == eventMsg.EventId)
                 .ExecuteAffrows();

                throw;
            }

            //4. 业务执行成功，发送 done 消息
            var doneMsg = new EventMessage
            {
                EventId = eventMsg.EventId,
                Status = MessageStatus.Done
            };

            using var resp = "http://localhost:5000/api/message"
                     .AsHttpClient()
                     .Config(new RequestOptions
                     {
                         ContentType = "application/json"
                     })
                     .Post(doneMsg);
            if (resp.Exception != null)
            {
                throw resp.Exception;
            }
            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("send done message to agile_dt_server fail .");
            }

            return new
            {
                success = true
            };
        }
    }
}
