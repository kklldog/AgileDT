using AgileDT.Data;
using AgileDT.Data.Entities;
using AgileDT.MessageQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileDT.Services.handlers
{
    public class MessageHandler_Done : IMessageStatusHandler
    {
        public MessageStatus Status => MessageStatus.Done;

        public void Handler(EventMessage message)
        {
            var msg = FreeSQL.Instance.Select<EventMessage>(new { EventId = message.EventId }).ToOne();

            if (msg == null)
            {
                throw new Exception($"Can not find eventmessage by id {message.EventId}");
            }

            //直接设置为状态为 waitsend ，数据库里其实不需要 done 这个状态
            FreeSQL.Instance.Update<EventMessage>()
                .Set(x => x.Status, MessageStatus.WaitSend)
                .Set(x => x.BizMsg, message.BizMsg)
                .Where(x => x.EventId == message.EventId)
                .ExecuteAffrows();
            //try to send msg to mq
            msg = FreeSQL.Instance.Select<EventMessage>().Where(x => x.EventId == message.EventId).ToOne();
            SendMsgToMQAndUpdateStatusSent(msg);
        }

        public void SendMsgToMQAndUpdateStatusSent(EventMessage msg)
        {
            FreeSQL.Instance.Ado.Transaction(() =>
            {
                FreeSQL.Instance.Update<EventMessage>().Set(x => x.Status, MessageStatus.Sent).Where(x => x.EventId == msg.EventId)
                                .ExecuteAffrows();
                SendMsgToMQ(msg);
            });
        }

        private void SendMsgToMQ(EventMessage msg)
        {
            MQ.Push(msg, msg.EventName);
        }
    }
}
