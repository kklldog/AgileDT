using AgileDT.Client;
using AgileDT.Client.Data;
using MemberService.Data;
using MemberService.Data.entities;
using Newtonsoft.Json;
using System;

namespace MemberService.Services
{
    [DtEventName("orderservice.order_added")]
    public class OrderAddedMessageHandler: IEventMessageHandler
    {
        static object _lock = new object();

        public bool Receive(EventMessage message)
        {
            var bizMsg = message.BizMsg;
            var eventId = message.EventId;
            string orderId = bizMsg;

            lock (_lock)
            {
                var entity = FreeSQL.Instance.Select<PointHistory>().Where(x => x.EventId == eventId).First();
                if (entity == null)
                {
                    var ret = FreeSQL.Instance.Insert(new PointHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        EventId = message.EventId,
                        OrderId = orderId,
                        Points = 20,
                        CreateTime = DateTime.Now
                    }).ExecuteAffrows();

                    return ret > 0;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
