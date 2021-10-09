using AgileDT.Client.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgileDT.Client
{
    public interface IEventMessageHandler
    {

        bool Receive(EventMessage message);
    }
}
