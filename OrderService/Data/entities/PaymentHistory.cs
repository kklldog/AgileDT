using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Data.entities
{
    [Table(Name = "payment_history")]
    public class PaymentHistory
    {
        [Column(Name = "id", StringLength = 36, IsPrimary = true)]
        public string Id { get; set; }
   
        [Column(Name = "event_id")]
        public string EventId { get; set; }
    }
}
