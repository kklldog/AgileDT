using AgileDT.Data;
using AgileDT.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileDT.Services.handlers
{
    public class MessageHandler_Cancel : IMessageStatusHandler
    {
        public MessageStatus Status => MessageStatus.Cancel;

        public void Handler(EventMessage message)
        {
            var msg = FreeSQL.Instance.Select<EventMessage>(new { EventId = message.EventId }).ToOne();

            if (msg == null)
            {
                throw new Exception($"Can not find eventmessage by id {message.EventId}");
            }

            FreeSQL.Instance.Update<EventMessage>().Set(x => x.Status, MessageStatus.Cancel).Where(x => x.EventId == message.EventId)
                .ExecuteAffrows();
        }
    }
}
