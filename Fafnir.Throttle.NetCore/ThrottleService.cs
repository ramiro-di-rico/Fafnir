using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace Fafnir.Throttle.NetCore
{
    public class ThrottleService : IThrottleService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ThrottleConfiguration throttleConfiguration;

        public ThrottleService(IMemoryCache memoryCache, IOptions<ThrottleConfiguration> throttleOptionConfig)
        {
            throttleConfiguration = throttleOptionConfig.Value;
            _memoryCache = memoryCache;
            Clients = memoryCache.GetOrCreate(nameof(ThrottleService), x => new List<ClientAddress>());
        }

        internal List<ClientAddress> Clients { get; }

        public bool IsAllowed(string address)
        {
            if (Clients.Any(x => x.Address.Equals(address)))
            {
                var client = Clients.First(x => x.Address.Equals(address));

                if (client.EndBan(throttleConfiguration.PenaltyTime) || client.ShouldClear(throttleConfiguration.Period))
                {
                    client.Clear();
                }

                client.IncreaseCounter();

                if (client.RquestsCount > throttleConfiguration.MaxRequests) client.Ban();

                if (client.IsBan) return false;
            }
            else
            {
                AddClient(address);
            }

            SaveTable();

            return true;
        }

        private void AddClient(string address)
        {
            var client = new ClientAddress { Address = address };
            client.IncreaseCounter();
            Clients.Add(client);
        }

        private void SaveTable()
        {
            _memoryCache.Set(nameof(ThrottleService), Clients);
        }
    }
}
