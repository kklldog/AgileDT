using AgileHttp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderService.Data;
using OrderService.Data.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AgileDT.Client;
using OrderService.Services;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IAddOrderService _addOrderService;

        public OrderController(ILogger<OrderController> logger, IAddOrderService addOrderService)
        {
            _addOrderService = addOrderService;
            _logger = logger;
        }

        [HttpPost]
        public object Add([FromBody] Order order)
        {
            //3.do bizz
            _addOrderService.AddOrder(order);

            return new
            {
                success = true
            };
        }
    }
}
