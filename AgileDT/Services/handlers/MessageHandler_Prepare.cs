using AgileDT.Data;
using AgileDT.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileDT.Services.handlers
{
    public class MessageHandler_Prepare : IMessageStatusHandler
    {
        public MessageStatus Status => MessageStatus.Prepare;

        public void Handler(EventMessage message)
        {
            message.Status = MessageStatus.Prepare;
            FreeSQL.Instance.Insert(message).ExecuteAffrows();
        }
    }
}
