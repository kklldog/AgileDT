# AgileDT
可靠消息最终一致性服务

## 运行服务端
## 安装客户端
## 主动方使用方法
1. 实现IEventService方法
处理主动方业务逻辑的类需要实现IEventService接口，并且标记那个方法是真正的业务方法。AgileDT在启动的时候会扫描这些类型，并且使用AOP技术生成代理类，在业务方法前后插入对应的逻辑来跟可靠消息服务通讯。
```
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
```
## 被动方使用方法
被动方需要接收MQ投递过来的消息，这些处理类需要实现IEventMessageHandler接口。AgileDT启动的时候会去扫描这些类，然后跟MQ建立绑定关系。
```
    [DtEventName("orderservice.order_added")]
    public class OrderAddedMessageHandler: IEventMessageHandler
    {
        static object _lock = new object();

        public bool Receive(EventMessage message)
        {
            var bizMsg = message.BizMsg;
            var eventId = message.EventId;
            string orderId = bizMsg;

            lock (_lock)
            {
                var entity = FreeSQL.Instance.Select<PointHistory>().Where(x => x.EventId == eventId).First();
                if (entity == null)
                {
                    var ret = FreeSQL.Instance.Insert(new PointHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        EventId = message.EventId,
                        OrderId = orderId,
                        Points = 20,
                        CreateTime = DateTime.Now
                    }).ExecuteAffrows();

                    return ret > 0;
                }
                else
                {
                    return true;
                }
            }
        }
    }
```
