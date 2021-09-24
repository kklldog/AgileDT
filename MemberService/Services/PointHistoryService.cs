using MemberService.Data;
using MemberService.Data.entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberService.Services
{
    public class PointHistoryService
    {
        static object _lock = new object();

        public bool HanldeMessage(string msg)
        {
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(msg);
            string bizzMsg = obj.BizzMsg;
            string eventId = obj.EventId;
            dynamic bizz = JsonConvert.DeserializeObject<dynamic>(bizzMsg);

            string orderId = bizz.orderId;
            string memberId = bizz.memberId;
            int point = bizz.point;

            lock (_lock)
            {
                var entity = FreeSQL.Instance.Select<PointHistory>().Where(x => x.EventId == eventId).First();
                if (entity == null)
                {
                    var ret = FreeSQL.Instance.Insert(new PointHistory { 
                        Id = Guid.NewGuid().ToString(),
                        EventId = eventId,
                        OrderId = orderId,
                        MemberId = memberId,
                        Points = point,
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
