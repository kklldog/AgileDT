using System;
using System.Collections.Generic;
using System.Text;

namespace AgileDT.Client.Sgr
{
    public class QueryStatusMessageHandler : MessageHandler
    {
        public QueryStatusMessageHandler() : base("QueryStatus")
        {
        }

        public override void Handle(string message)
        {
            Console.WriteLine("QueryStatusMessageHandler handle message " + message);
        }
    }
}
