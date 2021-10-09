using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileDT.Hubs
{
    public class AgileDtClient
    {
        public string Id { get; set; }

        public string GroupName { get; set; }
    }

    public class ClientCollection
    {
        private static ConcurrentDictionary<string, AgileDtClient> _clientMap = new ConcurrentDictionary<string, AgileDtClient>();

        public static void Add(AgileDtClient client)
        {
            _clientMap.TryAdd(client.Id, client);
        }

        public static void Remove(string clientId)
        {
            _clientMap.TryRemove(clientId, out AgileDtClient client);
        }

        public static AgileDtClient Get(string clientId)
        {
            _clientMap.TryGetValue(clientId, out AgileDtClient client);

            return client;
        }

        public static List<AgileDtClient> GetGroupClients(string group)
        {
            return _clientMap.Values.Where(x => x.GroupName == group).ToList();
        }
    }
}
