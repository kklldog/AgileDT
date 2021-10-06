
//using Microsoft.Extensions.Logging;
//using OrderService.Services;
//using OrderService.Data.entities;
//using System;

//using System;
//using AgileDT.Client;
//namespace OrderService.Services
//{
//    public class AddOrderEventService_agiledt_proxy : AddOrderEventService
//    {

//        public AddOrderEventService_agiledt_proxy(ILogger<AddOrderEventService> logger) : base(logger) { }


//        public override Boolean AddOrder(Order order)
//        {
//            var atr = Helper.GetDtEventBizMethodAttribute(typeof(AddOrderEventService));
//            atr.Before();

//            Boolean ret;
//            try
//            {
//                ret = base.AddOrder(order);
//            }
//            catch
//            {
//                const string sql = "update [EVENT_MESSAGE] set [STATUS] = @status where event_id = @id ";
//                FREESQL.Instance.Ado.ExecuteNonQuery(sql, new
//                {
//                    id = EventId,
//                    status = MessageStatus.Cancel
//                });

//                throw;
//            }

//            atr.After();

//            return ret;
//        }

//    }
//}
