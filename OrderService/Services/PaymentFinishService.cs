
using Microsoft.Extensions.Logging;
using OrderService.Services;
using OrderService.Data.entities;
using System;

using System;
using AgileDT.Client;
using OrderService.Data;
using AgileDT.Client.Data;
using Newtonsoft.Json;

namespace OrderService.Services
{
    public interface IPaymentFinishService : IEventService
    {
        void PayFinish(string payId);
    }

    [DtEventName("OrderService.PaymentFinish")]
    public class PaymentFinishService : IPaymentFinishService
    {
        public string EventId { get; set; }

        public string GetBizMsg()
        {
            var pay = FreeSQL.Instance.Select<PaymentHistory>().Where(x => x.EventId == EventId).First();
            var msg = new
            {
                payId = pay.Id,
                amount = 100,
                points = 1000
            };

            return JsonConvert.SerializeObject(msg);
        }

        [DtEventBizMethod]
        public virtual void PayFinish(string payId)
        {
            //3. 写 paymenthistory 跟 修改 event 的状态必选写在同一个事务内
            FreeSQL.Instance.Ado.Transaction(() =>
            {
               //在paymenthistory表新增一个eventid字段，使paymenthistory跟event_message表关联起来
                var ret0 = FreeSQL.Instance.Insert(new PaymentHistory { Id=Guid.NewGuid().ToString(),EventId = this.EventId }).ExecuteAffrows();
                var ret1 = FreeSQL.Instance.Update<OrderService.Data.entities.EventMessage>()
                .Set(x => x.Status, MessageStatus.Done)
                .Where(x => x.EventId == EventId)
                .ExecuteAffrows();
            });
        }
    }
}
