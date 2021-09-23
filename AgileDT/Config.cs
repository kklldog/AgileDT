using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileDT
{
    public class Config
    {
        public static IConfiguration Instance { get; set; }
    }
}
