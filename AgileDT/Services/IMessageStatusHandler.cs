using AgileDT.Data.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileDT.Services
{
    public interface IMessageStatusHandler
    {
        MessageStatus Status { get; }

        void Handler(EventMessage message);
    }
}
