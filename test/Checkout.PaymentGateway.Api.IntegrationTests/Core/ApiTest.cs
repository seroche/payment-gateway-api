using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using Alba;
using Xunit;

namespace Checkout.PaymentGateway.Api.IntegrationTests.Core
{
    [Collection(nameof(WebAppCollection))]
    public abstract class ApiTest<TRequest>
        where TRequest : class, new()
    {
        protected SystemUnderTest SystemUnderTest { get; }
        protected string EndpointRelativeUrl { get; }

        protected ApiTest(WebAppFixture fixture, string endpointRelativeUrl)
        {
            SystemUnderTest = fixture.SystemUnderTest;
            EndpointRelativeUrl = endpointRelativeUrl;
        }

        #region HttpRequest

        public async Task<Response<TResult>> Get<TResult>(TRequest request, string apiKey)
            => await Get<TRequest, TResult>(request, EndpointRelativeUrl, apiKey);

        public async Task<Response<TResult>> Get<T, TResult>(T request, string endpoint, string apiKey)
            => await SystemUnderTest.Request<T, TResult>(HttpMethod.Get, endpoint, request, apiKey);

        public async Task<Response> Post(TRequest request, string apiKey) 
            => await Post<TRequest, Response<Empty>>(request, EndpointRelativeUrl, apiKey);

        public async Task<Response<TResult>> Post<TResult>(TRequest request, string apiKey)
            => await Post<TRequest, TResult>(request, EndpointRelativeUrl, apiKey);

        public async Task<Response> Post<T>(T request, string endpoint, string apiKey)
            => await Post<T, Response<Empty>>(request, endpoint, apiKey);

        public async Task<Response<TResult>> Post<T, TResult>(T request, string endpoint, string apiKey)
            => await SystemUnderTest.Request<T, TResult>(HttpMethod.Post, endpoint, request, apiKey);
        #endregion
    }
}