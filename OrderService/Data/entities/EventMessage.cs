using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Data.entities
{
    public enum MessageStatus
    {
        Cancel = -1,
        Prepare = 0,
        Done = 1,
    }

    [Table(Name = "event_message")]
    public class EventMessage
    {
        [Column(Name = "event_id", StringLength = 36, IsPrimary = true)]
        public string EventId { get; set; }

        [Column(Name = "bizz_msg", StringLength = 4000)]
        public string BizzMsg { get; set; }

        [Column(Name = "status")]
        public MessageStatus Status { get; set; }

        [Column(Name = "query_api", StringLength = 1000)]
        public string QueryApi { get; set; }

        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }
    }
}
