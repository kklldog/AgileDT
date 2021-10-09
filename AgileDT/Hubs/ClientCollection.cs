using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileDT.Hubs
{
    public class ClientCollection
    {
        private static Dictionary<string, List<string>> _clientMap = new Dictionary<string, List<string>>();
        private static object _lock = new object();
        public static void Add(string eventName, string connectionId)
        {
            lock (_lock)
            {
                if (_clientMap.ContainsKey(eventName))
                {
                    var arr = _clientMap[eventName];
                    if (arr.Contains(connectionId))
                    {
                        return;
                    }
                    else
                    {
                        arr.Add(connectionId);
                    }
                }
                else
                {
                    _clientMap[eventName] = new List<string> { connectionId };
                }
            }
        }

        public static void Remove(string connectionId)
        {
            lock (_lock)
            {
                foreach (var lst in _clientMap.Values)
                {
                    if (lst.Contains(connectionId))
                    {
                        lst.Remove(connectionId);
                        break;
                    }
                }
            }
        }

        public static List<string> GetGroupClients(string group)
        {
            if (_clientMap.ContainsKey(group))
            {
                return _clientMap[group];
            }

            return new List<string>();
        }
    }
}
