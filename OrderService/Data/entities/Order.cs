using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Data.entities
{
    [Table(Name = "orders")]
    public class Order
    {
        [Column(Name = "id", StringLength = 36, IsPrimary = true)]
        public string Id { get; set; }
        [Column(Name = "member_id")]
        public string MemberId { get; set; }
        [Column(Name = "product_id")]
        public string ProductId { get; set; }
        [Column(Name = "price")]
        public int Price { get; set; }
        [Column(Name = "event_id")]
        public string EventId { get; set; }
    }
}
