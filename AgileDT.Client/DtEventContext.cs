using System;
using System.Collections.Generic;
using System.Text;

namespace AgileDT.Client
{
    public class DtEventContext
    {
        public DtEventContext(IEventService service)
        {
            EventId = Guid.NewGuid().ToString();
            Service = service;
            Service.EventId = EventId;
        }

        public string EventId { get; }

        public IEventService Service { get; }
    }
}
