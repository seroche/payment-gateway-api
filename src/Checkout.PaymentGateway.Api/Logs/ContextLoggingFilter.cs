using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Api.Logs
{
    [SuppressMessage("Microsoft.Performance", "CA1812")]
    internal class ContextLoggingFilter : IAsyncActionFilter
    {
        private static readonly string[] ExcludedTypes = { "CancellationToken" };

        private readonly ILoggerFactory _loggerFactory;
        public ContextLoggingFilter(ILoggerFactory loggerFactory)
            => _loggerFactory = loggerFactory;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var logger = _loggerFactory.CreateLogger(context.Controller.GetType());

            // List all incoming parameters so we could reproduce potential issues
            var args = context.ActionArguments?.Where(x => !ExcludedTypes.Contains(x.Value?.GetType().Name ?? ""));
            if (args is null)
                logger.LogInformation("No incoming payload.");
            else
                logger.LogInformation("Incoming payload is '{@ActionArguments}'.", args);

            var resp = await next();

            // List the result returned by the API
            logger.LogInformation("Result is {@Result}.", resp.Result);
        }
    }
}
