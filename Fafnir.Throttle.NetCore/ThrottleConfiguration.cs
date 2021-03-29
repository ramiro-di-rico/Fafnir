using System;

namespace Fafnir.Throttle.NetCore
{
    public class ThrottleConfiguration
    {
        public static string ConfigurationKey = nameof(ThrottleConfiguration);

        public int MaxRequests { get; set; } = 100;
        public TimeSpan Period { get; set; } = TimeSpan.FromMinutes(1);
        public TimeSpan PenaltyTime { get; set; } = TimeSpan.FromMinutes(5);
        public string ErrorMessage { get; set; } = "Too many requests.";
    }
}
