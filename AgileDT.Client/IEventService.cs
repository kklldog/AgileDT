using AgileDT.Client.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgileDT.Client
{
    public interface IEventService
    {
        string EventId { get; set; }

        string GetBizMsg();

        MessageStatus QueryEventStatus(string eventId)
        {
            var eventMsg = FREESQL.Instance.Select<EventMessage>().Where(x => x.EventId == eventId).First();

            if (eventMsg == null || eventMsg.Status == MessageStatus.Cancel)
            {
                return MessageStatus.Cancel;
            }

            if (eventMsg.Status == MessageStatus.Done)
            {
                return MessageStatus.Done;
            }

            if (eventMsg.Status == MessageStatus.Prepare)
            {
                if ((DateTime.Now - eventMsg.CreateTime.Value).TotalSeconds > 60)
                {
                    //如果 prepare 状态大于60s，直接认为已经取消
                    return MessageStatus.Cancel;
                }
            }

            return MessageStatus.Prepare;
        }
    }
}
