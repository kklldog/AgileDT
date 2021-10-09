using AgileDT.Client;
using AgileDT.Client.Data;
using Microsoft.Extensions.Logging;
using OrderService.Data;
using OrderService.Data.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Services
{
    public interface IAddOrderService:IEventService
    {
        bool AddOrder(Order order);
    }

    [DtEventName("orderservice.order_added")]
    public class AddOrderService : IAddOrderService
    {
        private readonly ILogger<AddOrderService> _logger;

        public AddOrderService(ILogger<AddOrderService> logger)
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
                order.EventId = EventId;//在订单表新增一个eventid字段，使order跟event_message表关联起来
                var ret0 = FreeSQL.Instance.Insert(order).ExecuteAffrows();
                var ret1 = FreeSQL.Instance.Update<OrderService.Data.entities.EventMessage>()
                .Set(x => x.Status, MessageStatus.Done)
                .Where(x => x.EventId == EventId)
                .ExecuteAffrows();

                ret = ret0 > 0 && ret1 > 0;
            });

            return ret;

        }

        /// <summary>
        /// 构造后续业务处理需要的消息内容
        /// </summary>
        /// <returns></returns>
        public string GetBizMsg()
        {
            //这里可以构造传递到MQ的业务消息的内容，比如传递订单编号啊 ,以便后续的被动方处理业务时候使用
            var order = FreeSQL.Instance.Select<Order>().Where(x => x.EventId == EventId).First();
            return order?.Id;
        }
      
    }
}
