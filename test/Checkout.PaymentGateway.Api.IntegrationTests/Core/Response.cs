using Checkout.PaymentGateway.Api.IntegrationTests.Core.Assertions;
using Microsoft.AspNetCore.Http;

namespace Checkout.PaymentGateway.Api.IntegrationTests.Core
{
    public sealed class Empty { }  // API calls returning empty bodies
    
    public class Response
    {
        public HttpContext Context { get; private set; }

        public Response(HttpContext context) => Context = context;

        public Assertion Assert()
            => new Assertion(Context.Response);
    }

    public class Response<TResult> : Response
    {
        public TResult Result { get; private set; }

        public Response(HttpContext context, TResult result)
            : base(context)
            => Result = result;

        public new Assertion<TResult> Assert()
            => new Assertion<TResult>(Result, Context.Response);
    }
}