using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgileDT.models;
using AgileDT.Services;
using AgileDT.Data.Entities;
using Newtonsoft.Json;

namespace AgileDT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;

        public MessageController(ILogger<MessageController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public ActionResult<object> Post([FromBody]EventMessageVM model)
        {
            _logger.LogInformation($"DT message controller receive message {JsonConvert.SerializeObject(model)}");

            if (model.Status == MessageStatus.Prepare 
                || model.Status == MessageStatus.Cancel 
                || model.Status == MessageStatus.Finish 
                || model.Status == MessageStatus.Done)
            {
                var handler = MessageHandler.GetHandler(model.Status);
                handler.Handler(new EventMessage
                {
                    BizMsg = model.BizMsg,
                    EventId = model.EventId,
                    Status = model.Status,
                    CreateTime = model.CreateTime,
                    EventName = model.EventName
                });

                return new
                {
                    success = true
                };
            }

            return new
            {
                success = false,
                message = $"can not handle message status {model.Status}"
            };
        }
    }
}
