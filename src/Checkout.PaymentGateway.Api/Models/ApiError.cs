using System;
using Checkout.PaymentGateway.Domain.Core;
using Microsoft.AspNetCore.Http;

namespace Checkout.PaymentGateway.Api.Models
{
    /// <summary>
    /// Error returned to the API clients
    /// </summary>
    public class ApiError
    {
        public string Code { get; set; }
        public string RequestId { get; set; }
        public DateTimeOffset Date { get; set; }

        public ApiError() { }

        /// <summary>
        /// Creates an instance of <see cref="ApiError"/>.
        /// </summary>
        /// <param name="error"><see cref="Error"/>.</param>
        /// <param name="requestId">The request identifier to extract from the <see cref="HttpContext"/>.</param>
        public ApiError(Error error, string requestId)
        {
            Code = error?.Code ?? string.Empty;
            Date = DateTimeOffset.UtcNow;
            RequestId = requestId;
        }
    }
}