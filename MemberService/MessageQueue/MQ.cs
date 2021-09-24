using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemberService.MessageQueue
{
    public static class MQ
    {
        static ConnectionFactory ConnectionFactory;
        static IConnection Connection;
        static IModel Model;
        static EventingBasicConsumer Consumer;
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
            Connection = ConnectionFactory.CreateConnection();
            Model = Connection.CreateModel();
            Model.ExchangeDeclare(ExName, ExchangeType.Direct, false, false, null);
            Model.QueueDeclare(QueueName, false, false, false, null);
            Model.QueueBind(QueueName, ExName, QueueKey, null);

            Consumer = new EventingBasicConsumer(Model);
            Model.BasicConsume(QueueName, false, Consumer);
        }

        public static void Push(string json)
        {
            var sendBytes = Encoding.UTF8.GetBytes(json);
            Model.BasicPublish(ExName, QueueKey, null, sendBytes);
        }

        public static void Receive(Action<IModel,BasicDeliverEventArgs> act)
        {
            Consumer.Received += (ch, args) =>
            { 
                act?.Invoke(Model, args);
            };
        }
    }
}
