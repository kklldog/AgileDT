using AgileDT.Client;
using AgileDT.Client.Data;
using MemberService.Data;
using MemberService.Data.entities;
using Newtonsoft.Json;
using System;

namespace MemberService.Services
{
    public interface IPaymentFinishMessageHandler : IEventMessageHandler
    {
    }

    [DtEventName("OrderService.PaymentFinish")]
    public class PaymentFinishMessageHandler : IEventMessageHandler
    {
        static object _lock = new object();

        public bool Receive(EventMessage message)
        {
            var bizMsg = message.BizMsg;
            var eventId = message.EventId;
            dynamic msg = JsonConvert.DeserializeObject<dynamic>(bizMsg);

            string payId = msg.payId;
            int amount = msg.amount;
            int points = msg.points;

            lock (_lock)
            {
                var entity = FreeSQL.Instance.Select<PointHistory>().Where(x => x.EventId == eventId).First();
                if (entity == null)
                {
                    var ret = FreeSQL.Instance.Insert(new PointHistory
                    {
                        Id = Guid.NewGuid().ToString(),
                        EventId = message.EventId,
                        Points = points,
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
