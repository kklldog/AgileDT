using AgileDT.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileDT.models
{
    public class EventMessageVM
    {
        public string EventId { get; set; }

        public string BizMsg { get; set; }

        public MessageStatus Status { get; set; }

        public DateTime? CreateTime { get; set; }

        public string EventName { get; set; }
    }
}
