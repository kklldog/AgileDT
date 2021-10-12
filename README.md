![GitHub stars](https://img.shields.io/github/stars/kklldog/AgileDT)
![Commit Date](https://img.shields.io/github/last-commit/kklldog/AgileDT/master.svg?logo=github&logoColor=green&label=commit)
![Nuget](https://img.shields.io/nuget/v/AgileDT.Client?label=AgileDT.Client)
![Docker image](https://img.shields.io/docker/v/kklldog/agile_dt?label=docker%20image)
# AgileDT
分布式事务-可靠消息最终一致性框架
## 依赖组件
+ mysql
+ rabbitmq
## 运行服务端
在服务新建一个数据库并且新建一张表
```
// crate event_message table on mysql
create table if not exists event_message
(
	event_id varchar(36) not null
		primary key,
	biz_msg varchar(4000) null,
	status enum('Prepare', 'Done', 'WaitSend', 'Sent', 'Finish', 'Cancel') not null,
	create_time datetime(3) null,
	event_name varchar(255) null
);
```
使用docker-compose运行服务端
```
version: "3"  # optional since v1.27.0
services:
  agile_dt:
    image: "kklldog/agile_dt"
    ports:
      - "5000:5000"
    environment:
      - db:provider=mysql
      - db:conn= Database=agile_dt;Data Source=192.168.0.115;User Id=root;Password=mdsd;port=3306
      - mq:userName=admin
      - mq:password=123456
      - mq:host=192.168.0.115
      - mq:port=5672
```
## 安装客户端
在主动方跟被动方都需要安装AgileDT的客户端库
```
Install-Package AgileDT.Client
```
## 主动方使用方法
1. 在业务数据库添加事务消息表
```
// crate event_message table on mysql
create table if not exists event_message
(
	event_id varchar(36) not null
		primary key,
	biz_msg varchar(4000) null,
	status enum('Prepare', 'Done', 'WaitSend', 'Sent', 'Finish', 'Cancel') not null,
	create_time datetime(3) null,
	event_name varchar(255) null
);

```
2. 修改配置文件
```
在appsettings.json文件添加以下节点：
  "agiledt": {
    "server": "http://localhost:5000",
    "db": {
      "provider": "mysql",
      "conn": "Database=agile_order;Data Source=192.168.0.125;User Id=dev;Password=dev@123f;port=13306"
      //"conn": "Database=agile_order;Data Source=192.168.0.115;User Id=root;Password=mdsd;port=3306"
    },
    "mq": {
      "host": "192.168.0.125",
      //"host": "192.168.0.115",
      "userName": "admin",
      "password": "123456",
      "port": 5672
    }
  }
```
3. 注入 AgileDT 客户端服务
```
       public void ConfigureServices(IServiceCollection services)
        {
            services.AddAgileDT();
            ...
        }
```
5. 实现IEventService方法   
处理主动方业务逻辑的类需要实现IEventService接口，并且标记那个方法是真正的业务方法。AgileDT在启动的时候会扫描这些类型，并且使用AOP技术生成代理类，在业务方法前后插入对应的逻辑来跟可靠消息服务通讯。
这里要注意的几个地方：
+ 实现IEventService接口
+ 使用DtEventBizMethod注解标记业务入口方法
+ 使用DtEventName注解来标记事务的方法名称，如果不标记则使用类名
   
> 注意：业务方法最终一定要使用事务来同步修改消息表的status字段为done状态，这个操作框架没办法帮你实现   
> 注意：业务方法如果失败请抛出Exception，如果不抛异常框架一律认为执行成功
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
1. 在业务方数据库建表或者在业务表上加字段   
对于被动方来说这里不是必须要建一个表。但是至少要有个地方来存储event_id的信息，最简单的是直接在业务主表上加event_id字段。
2. 修改配置文件
```
在appsettings.json文件添加以下节点：
  "agiledt": {
    "server": "http://localhost:5000",
    "db": {
      "provider": "mysql",
      "conn": "Database=agile_order;Data Source=192.168.0.125;User Id=dev;Password=dev@123f;port=13306"
      //"conn": "Database=agile_order;Data Source=192.168.0.115;User Id=root;Password=mdsd;port=3306"
    },
    "mq": {
      "host": "192.168.0.125",
      //"host": "192.168.0.115",
      "userName": "admin",
      "password": "123456",
      "port": 5672
    }
  }
```
3. 注入AgileDT服务
```
       public void ConfigureServices(IServiceCollection services)
        {
            services.AddAgileDT();
            ...
        }
```
4. 实现IEventMessageHandler接口   
被动方需要接收MQ投递过来的消息，这些处理类需要实现IEventMessageHandler接口。AgileDT启动的时候会去扫描这些类，然后跟MQ建立绑定关系。
+ 这里必须使用DtEventName注解标记需要处理的事件名称
+ Reveive 方法必须是冥等的
```
    public interface IOrderAddedMessageHandler: IEventMessageHandler
    {
    }
    
    [DtEventName("orderservice.order_added")]
    public class OrderAddedMessageHandler: IOrderAddedMessageHandler
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
