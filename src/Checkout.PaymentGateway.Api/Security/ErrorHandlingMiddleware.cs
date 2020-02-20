using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Api.Models;
using Checkout.PaymentGateway.Domain;
using Checkout.PaymentGateway.Domain.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Checkout.PaymentGateway.Api.Security
{
    // Note: Prefer middleware for exception handling.
    // src: https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-2.2#exception-filters
    [SuppressMessage("Microsoft.Performance", "CA1812")]
    internal class ErrorHandlingMiddleware
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatString = "yyyy-MM-ddTHH:mm:ss"
        };

        private static readonly int[] ErrorStatusCodes =
        {
            (int)HttpStatusCode.Unauthorized,
            (int)HttpStatusCode.NotFound
        };

        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var response = context.Response;
            try
            {
                await _next.Invoke(context);
                if (!ErrorStatusCodes.Contains(response.StatusCode)
                    || response.HasStarted
                    || !(response.ContentType is null))
                    return;

                // Return a standardized error when clients try to access endpoints that do exist
                await WriteErrorAsync(context, Errors.UnauthorizedAccess, (int)HttpStatusCode.Unauthorized);
            }
            catch (Exception e)
            {
                // In many different scenario, Request can be cancelled. Hence, we should not log this exception as error
                if (context.RequestAborted.IsCancellationRequested)
                    _logger.LogWarning(e, "{ExceptionType} was thrown from '{Path}' due to an aborted request.", e.GetType().Name, context.Request.Path);
                else
                    _logger.LogError(e, "{ExceptionType} was thrown from '{Path}'.", e.GetType().Name, context.Request.Path);

                if (response.HasStarted) throw; // Note: cannot write to response if HasStarted is set to true
                await WriteErrorAsync(context, new UnexpectedError(e.GetBaseException()), 500);
            }
        }

        private static Task WriteErrorAsync(HttpContext context, Error error, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(
                JsonConvert.SerializeObject(
                    new ApiError(error, context.TraceIdentifier),
                    SerializerSettings));
        }
    }
}