using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Fafnir.Throttle.NetCore
{
    public static class ThrottleInitializer
    {
        public static IServiceCollection AddThrottle(this IServiceCollection services)
        {
            return services.AddScoped<IThrottleService, ThrottleService>();
        }

        public static IApplicationBuilder UseThrottle(
                this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ThrottleServiceMiddleware>();
        }
    }
}
