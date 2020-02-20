using System;
using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace Checkout.PaymentGateway.Api.Logs
{
    internal class LogEventEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _accessor;

        public LogEventEnricher(IHttpContextAccessor accessor) => _accessor = accessor;

        public void Enrich(LogEvent log, ILogEventPropertyFactory factory)
        {
            if (log is null) throw new ArgumentNullException(nameof(log));
            if (factory is null) throw new ArgumentNullException(nameof(factory));

            var context = _accessor.HttpContext;
            if (context is null) return;

            var request = context.Request;

            log.AddOrUpdateProperty(
                factory.CreateProperty("Method", request.Method));

            log.AddOrUpdateProperty(
                factory.CreateProperty("Host", $"{request.Host}".ToLowerInvariant()));

            log.AddOrUpdateProperty(
                factory.CreateProperty("RequestPath", $"{request.Path}".ToLowerInvariant()));

            log.AddOrUpdateProperty(
                factory.CreateProperty("QueryString", $"{request.QueryString}".ToLowerInvariant()));

            log.AddOrUpdateProperty(
                factory.CreateProperty("UserAgent", $"{request.Headers["User-Agent"]}".ToLowerInvariant()));
            
            if (context.User?.Identity is { })
            {
                log.AddOrUpdateProperty(
                    factory.CreateProperty("IsAuthenticated", context.User.Identity.IsAuthenticated));

                log.AddOrUpdateProperty(
                    factory.CreateProperty("AuthenticationType", context.User.Identity.AuthenticationType));

                log.AddOrUpdateProperty(
                    factory.CreateProperty("MerchantId", context.User.FindFirst("MerchantId")?.Value));
            }
            
            log.RemovePropertyIfPresent("ActionId");
            log.RemovePropertyIfPresent("SpanId");
            log.RemovePropertyIfPresent("TraceId");
            log.RemovePropertyIfPresent("CorrelationId");
            log.RemovePropertyIfPresent("ConnectionId");
        }
    }
}