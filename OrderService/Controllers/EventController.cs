using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderService.Data;
using OrderService.Data.entities;
using OrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentFinishService _paymentFinishService;

        public PaymentController(IPaymentFinishService paymentFinishService)
        {
            _paymentFinishService = paymentFinishService;
        }


        [HttpPost]
        public IActionResult Post(string payId)
        {
            _paymentFinishService.PayFinish(payId);

            return Ok();
        }
    }
}
