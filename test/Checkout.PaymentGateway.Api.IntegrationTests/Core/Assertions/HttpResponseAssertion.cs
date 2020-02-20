using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Checkout.PaymentGateway.Api.IntegrationTests.Core.Assertions
{
    public class HttpResponseAssertion
    {
        private HttpResponse Response { get; }

        internal HttpResponseAssertion(HttpResponse response)
        {
            Response = response;
        }

        public HttpResponseAssertion HasStatusCode(HttpStatusCode statusCode)
        {
            Assert.Equal((int)statusCode, Response.StatusCode);
            return this;
        }
        
        public HttpResponseAssertion ContainsHeader(Predicate<KeyValuePair<string, StringValues>> predicate)
        {
            Assert.Contains(Response.Headers, predicate);
            return this;
        }
    }
}
