using Microsoft.Extensions.DependencyInjection;

namespace Fafnir.Throttle
{
    public static class ThrottleInitializer
    {
        public static IServiceCollection AddThrottleService(this IServiceCollection services)
        {
            return services.AddScoped<IThrottleService, ThrottleService>();
        }
    }
}
