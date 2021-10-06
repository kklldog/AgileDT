using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgileDT.Client
{
    internal class Config
    {
        public static IConfiguration Instance { get; set; }
    }
}
