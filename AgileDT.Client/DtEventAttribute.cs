using System;

namespace AgileDT.Client
{
    public enum MessageStatus
    {
        Cancel = -1,
        Prepare = 0,
        Done = 1,
        WaitSend = 2,
        Sent = 3,
        Finish = 4
    }

    public interface IEvent
    {
        string EventId { get; set; }

        MessageStatus QueryEventStatus(string eventId);
    }

    public class DtEventBizMethodAttribute : Attribute
    {
    }

}
