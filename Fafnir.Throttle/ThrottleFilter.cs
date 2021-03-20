using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Threading.Tasks;

namespace Fafnir.Throttle
{
    public class ThrottleFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var throttleService = context.HttpContext.RequestServices.GetService(typeof(IThrottleService)) as IThrottleService;

            var address = context.HttpContext.Connection.RemoteIpAddress.ToString();

            if (!throttleService.IsAllowed(address))
            {
                context.Result = new ContentResult { StatusCode = (int)HttpStatusCode.Forbidden, Content = "Too many requests." };

                return;
            }

            await next();
        }
    }
}
