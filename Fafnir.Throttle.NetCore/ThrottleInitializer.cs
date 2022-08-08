using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Fafnir.Throttle.NetCore
{
    public static class ThrottleInitializer
    {
        /// <summary>
        /// Adds throttle service to the specified IServiceCollection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
        public static IServiceCollection AddThrottle(this IServiceCollection services)
        {
            services.AddMemoryCache();
            return services.AddScoped<IThrottleService, ThrottleService>();
        }

        /// <summary>
        /// Adds throttle service to the specified IServiceCollection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="maxRequests">The amounts of valid requests between each period</param>
        /// <param name="period">Period to validate the amount of requests, it is refreshed when period expires</param>
        /// <param name="penaltyTime">How long the requester will be rejected</param>
        /// <param name="errorMessage">Custom message to diplsay to the requester</param>
        /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
        public static IServiceCollection AddThrottle(this IServiceCollection services, int? maxRequests, TimeSpan? period, TimeSpan? penaltyTime, string errorMessage = "Too many requests.")
        {
            services.Configure<ThrottleConfiguration>(configuration =>
            {
                configuration.MaxRequests = maxRequests ?? 100;
                configuration.Period = period ?? TimeSpan.FromMinutes(1);
                configuration.PenaltyTime = penaltyTime ?? TimeSpan.FromMinutes(5);
                configuration.ErrorMessage = errorMessage;
            });
            return AddThrottle(services);
        }

        /// <summary>
        /// Adds throttle service to the specified IServiceCollection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">An object that implements Microsoft.Extensions.Configuration.IConfiguration</param>
        /// <returns>The Microsoft.Extensions.DependencyInjection.IServiceCollection so that additional calls can be chained.</returns>
        public static IServiceCollection AddThrottle(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ThrottleConfiguration>(configuration.GetSection(ThrottleConfiguration.ConfigurationKey));
            return AddThrottle(services);
        }

        /// <summary>
        /// Adds ThrottleServiceMiddleware to the specified Microsoft.AspNetCore.Builder.IApplicationBuilder.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseThrottle(
                this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ThrottleServiceMiddleware>();
        }
    }
}
