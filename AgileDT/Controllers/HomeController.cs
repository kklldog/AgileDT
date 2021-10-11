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
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "AgileDT server is running now .";
        }
    }
}
