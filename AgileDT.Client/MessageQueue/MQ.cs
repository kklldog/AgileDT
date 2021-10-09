using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileDT.Client.MessageQueue
{
    public static class MQ
    {
        static ConnectionFactory ConnectionFactory;
        static IConnection Connection;
        static IModel Model;
        static List<EventingBasicConsumer> Consumers;
        const string ExName = "ex_agile_dt";

        static MQ()
        {
            Consumers = new List<EventingBasicConsumer>();
            int port = 5672;
            if (int.TryParse(Config.Instance["agiledt:mq:port"],out int p))
            {
                port = p;
            }
            ConnectionFactory = new ConnectionFactory
            {
                UserName = Config.Instance["agiledt:mq:userName"],//用户名
                Password = Config.Instance["agiledt:mq:password"],//密码
                HostName = Config.Instance["agiledt:mq:host"],//rabbitmq ip
                Port = port
            };
            Connection = ConnectionFactory.CreateConnection();
            Model = Connection.CreateModel();
            Model.ExchangeDeclare(ExName, ExchangeType.Direct, false, false, null);
        }

        public static void BindConsumer(string queue, Action<IModel, BasicDeliverEventArgs> act)
        {
            //定义一个队列
            Model.QueueDeclare(queue, false, false, false, null);
            //将队列绑定到交换机
            Model.QueueBind(queue, ExName, queue, null);

            var consumer = new EventingBasicConsumer(Model);
            Model.BasicConsume(queue, false, consumer);
            consumer.Received += (ch, args) =>
            {
                act?.Invoke(Model, args);
            };

            Consumers.Add(consumer);
        }
    }
}
