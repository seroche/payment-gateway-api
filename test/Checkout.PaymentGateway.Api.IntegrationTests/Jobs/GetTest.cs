using Checkout.PaymentGateway.Api.IntegrationTests.Core;
using Checkout.PaymentGateway.Api.Models;
using Checkout.PaymentGateway.Domain;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Api.Features.Jobs;
using Checkout.PaymentGateway.Api.IntegrationTests.Payments;
using Checkout.PaymentGateway.Api.Messaging;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Checkout.PaymentGateway.Api.IntegrationTests.Jobs
{
    public class GetTest : ApiTest<Get.Query>
    {
        public GetTest(WebAppFixture fixture)
            : base(fixture, "/jobs/{JobId}")
        { }

        [Theory]
        [InlineData("")]
        [InlineData("test")]
        public async Task Get_ShouldReturn401Error_WhenApiKeyIsMissingOrInvalid(string apiKey)
        {
            var query = new Get.Query
            {
                JobId = Guid.NewGuid()
            };

            (await Get<ApiError>(query, apiKey))
                .Assert()
                .HttpResponse(a => a.HasStatusCode(HttpStatusCode.Unauthorized))
                .Result(r => r.IsEqual(Errors.UnauthorizedAccess.Code, m => m.Code));
        }
        
        [Fact]
        public async Task Get_ShouldReturn404Error_WhenJobCannotNotBeFound()
        {
            var query = new Get.Query
            {
                JobId = Guid.NewGuid()
            };

            (await Get<ApiError>(query, Data.ValidApiKey))
                .Assert()
                .HttpResponse(a => a.HasStatusCode(HttpStatusCode.NotFound))
                .Result(r => r.IsEqual(Errors.UnknownJob.Code, m => m.Code));
        }

        [Fact]
        public async Task Get_ShouldReturnJob()
        {
            var jobId = (await Post(Data.ValidRequestPaymentCommand, "/payments", Data.ValidApiKey))
                .Context.Response.Headers[HeaderNames.Location].First().Replace("/jobs/", "");

            var query = new Get.Query
            {
                JobId = Guid.Parse(jobId)
            };

            (await Get<Get.Model>(query, Data.ValidApiKey))
                .Assert()
                .HttpResponse(a => a.HasStatusCode(HttpStatusCode.OK))
                .Result(r => r
                    .IsEqual($"{JobStatus.Created}", m => m.Status)
                    .IsNull(m => m.EntityId)
                    .IsNull(m => m.Error));
        }
    }
}