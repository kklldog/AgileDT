﻿using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    [Table(Name = "event_message")]
    internal class EventMessage
    {
        [Column(Name = "event_id", StringLength = 36, IsPrimary = true)]
        public string EventId { get; set; }

        [Column(Name = "biz_msg", StringLength = 4000)]
        public string BizMsg { get; set; }

        [Column(Name = "status")]
        public MessageStatus Status { get; set; }

        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }
    }
}
