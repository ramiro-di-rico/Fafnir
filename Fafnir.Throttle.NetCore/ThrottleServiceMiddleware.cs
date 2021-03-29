using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;
using System.Threading.Tasks;

namespace Fafnir.Throttle.NetCore
{
    public class ThrottleServiceMiddleware
    {
        private readonly RequestDelegate _next;

        public ThrottleServiceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IThrottleService throttleService, IOptions<ThrottleConfiguration> throttleOptions)
        {
            var address = httpContext.Connection.RemoteIpAddress.ToString();

            if (!throttleService.IsAllowed(address))
            {
                httpContext.Response.StatusCode = 429;

                await httpContext.Response.Body.WriteAsync(Encoding.ASCII.GetBytes(throttleOptions.Value.ErrorMessage));

                return;
            }

            await _next(httpContext);
        }
    }
}
