using AgileDT.Data.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileDT.models
{
    public class EventMessageVM
    {
        public string EventId { get; set; }

        public string BizzMsg { get; set; }

        public MessageStatus Status { get; set; }

        public string QueryApi { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
