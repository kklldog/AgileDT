using System;
using System.Collections.Generic;
using System.Text;

namespace AgileDT.Client.Sgr
{
    public abstract class MessageHandler
    {
        public string Type { get; }

        public MessageHandler(string type)
        {
            Type = type;
        }

        public abstract void Handle(string message);
    }
}
