using System;
using System.Collections.Generic;
using System.Text;

namespace AgileDT.Client.Sgr
{
    public interface IMessageHandler
    {
        string Type { get; }

        void Handle(string message);
    }
}
