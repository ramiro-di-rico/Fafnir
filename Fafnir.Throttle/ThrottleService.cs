using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fafnir.Throttle
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

        private void SaveTable()
        {
            _memoryCache.Set(nameof(ThrottleService), Clients);
        }

        public bool IsAllowed(string address)
        {
            if (Clients.Any(x => x.Address.Equals(address)))
            {
                var client = Clients.First(x => x.Address.Equals(address));

                var endBan = client.LastRequest < DateTime.Now.Subtract(throttleConfiguration.PenaltyTime);
                var periodClearing = client.LastRequest < DateTime.Now.Subtract(throttleConfiguration.Period);

                if (endBan || (periodClearing && !client.IsBan))
                {
                    client.Clear();
                }

                client.IncreaseCounter();

                if (client.RquestsCount > throttleConfiguration.MaxRequests) client.Ban();

                if (client.IsBan) return false;
            }
            else
            {
                var client = new ClientAddress { Address = address };
                client.IncreaseCounter();
                Clients.Add(client);
            }

            SaveTable();

            return true;
        }
    }
}
