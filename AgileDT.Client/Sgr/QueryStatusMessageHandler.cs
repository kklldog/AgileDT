using System;
using System.Collections.Generic;
using System.Text;

namespace AgileDT.Client.Sgr
{
    public class QueryStatusMessageHandler : IMessageHandler
    {
        public string Type => "QueryStatus";
        public void Handle(string message)
        {
            Console.WriteLine("QueryStatusMessageHandler handle message " + message);
        }
    }
}
