using AgileDT.Client;
using Microsoft.Extensions.Logging;
using OrderService.Data;
using OrderService.Data.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Services
{
    public interface IAddOrderService:IEvent
    {
        bool AddOrder(Order order);
    }

    public class AddOrderEventService : IAddOrderService
    {
        private readonly ILogger<AddOrderEventService> _logger;

        public AddOrderEventService(ILogger<AddOrderEventService> logger)
        {
            _logger = logger;
        }

        public string EventId { 
            get;
            set;
        }
        
        [DtEventBizMethod]
        public virtual bool AddOrder(Order order)
        {
            var ret = false;

            //3. 写 Order 跟 修改 event 的状态必选写在同一个事务内
            FreeSQL.Instance.Ado.Transaction(() =>
            {
                order.EventId = EventId;
                var ret0 = FreeSQL.Instance.Insert(order).ExecuteAffrows();
                var ret1 = FreeSQL.Instance.Update<EventMessage>()
                .Set(x => x.Status, MessageStatus.Done)
                .Where(x => x.EventId == EventId)
                .ExecuteAffrows();

                ret = ret0 > 0 && ret1 > 0;
            });

            return ret;

        }

        public MessageStatus QueryEventStatus(string eventId)
        {
            var eventMsg = FreeSQL.Instance.Select<EventMessage>().Where(x => x.EventId == eventId).First();

            if (eventMsg == null || eventMsg.Status == MessageStatus.Cancel)
            {
                return MessageStatus.Cancel;
            }

            if (eventMsg.Status == MessageStatus.Done)
            {
                return MessageStatus.Done;
            }

            if (eventMsg.Status == MessageStatus.Prepare)
            {
                if ((DateTime.Now - eventMsg.CreateTime.Value).TotalSeconds > 60)
                {
                    //如果 prepare 状态大于60s，直接认为已经取消
                    return MessageStatus.Cancel;
                }

                var order = FreeSQL.Instance.Select<Order>().Where(x => x.EventId == EventId).First();
                if (order == null)
                {
                    return MessageStatus.Cancel;
                }
            }

            return MessageStatus.Prepare;
        }
    }
}
