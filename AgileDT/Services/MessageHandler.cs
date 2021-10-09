using AgileDT.Data.Entities;
using AgileDT.Services.handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileDT.Services
{
    public static class MessageHandler
    {
        static Dictionary<MessageStatus, IMessageStatusHandler> _handlers;
        static MessageHandler()
        {
            _handlers = new Dictionary<MessageStatus, IMessageStatusHandler>();
            AddHandler(new MessageHandler_Prepare());
            AddHandler(new MessageHandler_Done());
            AddHandler(new MessageHandler_Cancel());
            AddHandler(new MessageHandler_Finish());
        }

        public static void AddHandler(IMessageStatusHandler handler)
        {
            _handlers.TryAdd(handler.Status, handler);
        }

        public static IMessageStatusHandler GetHandler (MessageStatus status)
        {
            _handlers.TryGetValue(status, out IMessageStatusHandler handler);

            if (handler == null)
            {
                throw new Exception($"Can not find the handler for status {status}");
            }

            return handler;
        }
    }
}
