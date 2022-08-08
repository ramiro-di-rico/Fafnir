using System;

namespace Fafnir.Throttle.NetCore
{
    public class ThrottleConfiguration
    {
        public static string ConfigurationKey = nameof(ThrottleConfiguration);

        /// <summary>
        /// The amounts of valid requests between each period
        /// </summary>
        public int MaxRequests { get; set; } = 100;

        /// <summary>
        /// Period to validate the amount of requests, it is refreshed when period expires
        /// </summary>
        public TimeSpan Period { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// How long the requester will be rejected
        /// </summary>
        public TimeSpan PenaltyTime { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Custom message to diplsay to the requester
        /// </summary>
        public string ErrorMessage { get; set; } = "Too many requests.";
    }
}
