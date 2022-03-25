using System;
using System.Linq;
using System.Threading.Tasks;
using Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TestTaskTracker.Filters
{
    public class XPassFilter : IAsyncActionFilter
    {
        /// <param name="context">Current context of request</param>
        /// <param name="next">What will be executing next</param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var requests = context.ActionArguments.Values;
            var xpass = requests.OfType<IXPass>().FirstOrDefault();
            if (xpass != null)
            {
                xpass.XPass = context.HttpContext.Request.Headers[nameof(xpass.XPass)];
            }

            await next();
        }
    }
}
