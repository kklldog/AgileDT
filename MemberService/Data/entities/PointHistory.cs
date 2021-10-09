using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberService.Data.entities
{

    [Table(Name = "point_history")]
    public class PointHistory
    {
        [Column(Name = "id", StringLength = 36, IsPrimary = true)]
        public string Id { get; set; }
        [Column(Name = "event_id", StringLength = 36)]
        public string EventId { get; set; }

        [Column(Name = "Order_Id", StringLength = 36)]
        public string OrderId { get; set; }

        [Column(Name = "points")]
        public int Points { get; set; }

        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }
    }
}
