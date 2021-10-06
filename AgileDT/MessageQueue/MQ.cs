using AgileDT.Data.Entites;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileDT.MessageQueue
{
    public static class MQ
    {
        static ConnectionFactory ConnectionFactory;
        static IConnection Connection;
        static IModel Model;

        const string ExName = "ex_agile_dt";
        const string QueueName = "q_agile_dt";
        const string QueueKey = "k_agile_dt";

        static MQ()
        {
            ConnectionFactory = new ConnectionFactory
            {
                UserName = Config.Instance["mq:userName"],//用户名
                Password = Config.Instance["mq:password"],//密码
                HostName = Config.Instance["mq:host"]//rabbitmq ip
            };
            //创建连接
            Connection  = ConnectionFactory.CreateConnection();
            //创建通道
            Model = Connection.CreateModel();
            //定义一个Direct类型交换机
            Model.ExchangeDeclare(ExName, ExchangeType.Direct, false, false, null);
            //定义一个队列
            Model.QueueDeclare(QueueName, false, false, false, null);

            //将队列绑定到交换机
            Model.QueueBind(QueueName, ExName, QueueKey, null);
        }

        public static void Push(EventMessage eventMesssage)
        {
            var json = JsonConvert.SerializeObject(new { 
                eventMesssage.BizMsg,
                eventMesssage.EventId
            });

            var sendBytes = Encoding.UTF8.GetBytes(json);
            //发布消息
            Model.BasicPublish(ExName, QueueKey, null, sendBytes);
        }
    }
}
