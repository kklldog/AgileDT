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
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<object> Add([FromBody]Order order)
        {
            //1. 往event_message表写 Prepare 数据
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
            FreeSQL.Instance.Insert(eventMsg).ExecuteAffrows();

            try
            {
                //2. 调用可靠消息服务的接口把 prepare 状态传递过去
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("http://localhost:5000");
                var content = new StringContent(JsonConvert.SerializeObject(eventMsg), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("/api/message", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "send message to agile_dt_server fail .");
                FreeSQL.Instance.Delete<EventMessage>()
                 .Where(x => x.EventId == eventMsg.EventId)
                 .ExecuteAffrows();
                throw;
            }

            try
            {
                //3. 写 Order 跟 修改 event 的状态必选写在同一个事务内
                FreeSQL.Instance.Ado.Transaction(() => {
                    order.EventId = eventMsg.EventId;
                    FreeSQL.Instance.Insert(order).ExecuteAffrows();
                    FreeSQL.Instance.Update<EventMessage>()
                    .Set(x => x.Status, MessageStatus.Done)
                    .Where(x => x.EventId == eventMsg.EventId)
                    .ExecuteAffrows();
                });
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
            var contentDone = new StringContent(JsonConvert.SerializeObject(new EventMessage { 
                EventId = eventMsg.EventId,
                Status = MessageStatus.Done
            }), Encoding.UTF8, "application/json");
            var httpClientDone = new HttpClient();
            httpClientDone.BaseAddress = new Uri("http://localhost:5000");
            var responseDone = await httpClientDone.PostAsync("/api/message", contentDone);
            responseDone.EnsureSuccessStatusCode();

            return new
            {
                success = true
            };
        }
    }
}
