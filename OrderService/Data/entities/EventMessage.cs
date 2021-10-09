using AgileDT.Client.Data;
using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Data.entities
{

    [Table(Name = "event_message")]
    public class EventMessage
    {
        [Column(Name = "event_id", StringLength = 36, IsPrimary = true)]
        public string EventId { get; set; }

        [Column(Name = "biz_msg", StringLength = 4000)]
        public string BizMsg { get; set; }

        [Column(Name = "status")]
        public MessageStatus Status { get; set; }

        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        [Column(Name = "event_name")]
        public string EventName { get; set; }
    }
}
