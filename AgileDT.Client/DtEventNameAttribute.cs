using AgileDT.Client.Data;
using AgileHttp;
using System;

namespace AgileDT.Client
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DtEventNameAttribute : Attribute
    {
        private string _eventName;
        public DtEventNameAttribute(string eventName)
        {
            _eventName = eventName;
        }

        public string EventName
        {
            get
            {
                return _eventName;
            }
        }
    }

}
