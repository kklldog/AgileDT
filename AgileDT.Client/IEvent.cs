using System;
using System.Collections.Generic;
using System.Text;

namespace AgileDT.Client
{
    public interface IEventService
    {
        string EventId { get; set; }

        string GetBizMsg();

        MessageStatus QueryEventStatus(string eventId);
    }
}
