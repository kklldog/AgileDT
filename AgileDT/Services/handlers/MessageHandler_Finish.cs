using AgileDT.Data;
using AgileDT.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileDT.Services.handlers
{
    public class MessageHandler_Finish : IMessageStatusHandler
    {
        public MessageStatus Status => MessageStatus.Finish;

        public void Handler(EventMessage message)
        {
            var msg = FreeSQL.Instance.Select<EventMessage>(new { EventId = message.EventId }).ToOne();

            if (msg == null)
            {
                throw new Exception($"Can not find eventmessage by id {message.EventId}");
            }

            if (msg.Status != MessageStatus.Sent)
            {
                throw new Exception($"Message status is {msg.Status} now , can not set finish status .");
            }

            FreeSQL.Instance.Update<EventMessage>().Set(x => x.Status, MessageStatus.Finish).Where(x => x.EventId == message.EventId)
                .ExecuteAffrows();
        }
    }
}
