using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Api.Features.Payments;
using Checkout.PaymentGateway.Api.IntegrationTests.Core;
using Checkout.PaymentGateway.Api.Models;
using Checkout.PaymentGateway.Domain;
using Checkout.PaymentGateway.Domain.Payments;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Checkout.PaymentGateway.Api.IntegrationTests.Payments
{
    public class GetTest : ApiTest<Get.Query>
    {
        public GetTest(WebAppFixture fixture)
            : base(fixture, "/payments/{PaymentId}")
        { }

        [Theory]
        [InlineData("")]
        [InlineData("test")]
        public async Task Get_ShouldReturn401Error_WhenApiKeyIsMissingOrInvalid(string apiKey)
        {
            var query = new Get.Query
            {
                PaymentId = $"{Guid.Empty}"
            };

            (await Get<ApiError>(query, apiKey))
                .Assert()
                .HttpResponse(a => a.HasStatusCode(HttpStatusCode.Unauthorized))
                .Result(r => r.IsEqual(Errors.UnauthorizedAccess.Code, m => m.Code));
        }

        [Fact]
        public async Task Get_ShouldReturn404Error_WhenPaymentCannotNotBeFound()
        {
            var query = new Get.Query
            {
                PaymentId = $"{Guid.NewGuid()}"
            };

            (await Get<ApiError>(query, Data.ValidApiKey))
                .Assert()
                .HttpResponse(a => a.HasStatusCode(HttpStatusCode.NotFound))
                .Result(r => r.IsEqual(Errors.UnknownPayment.Code, m => m.Code));
        }

        [Fact]
        public async Task Get_ShouldReturn400Error_WhenPaymentCannotNotBeFound()
        {
            var query = new Get.Query
            {
                PaymentId = $"{Guid.NewGuid()}"
            };

            (await Get<ApiError>(query, Data.ValidApiKey))
                .Assert()
                .HttpResponse(a => a.HasStatusCode(HttpStatusCode.NotFound))
                .Result(r => r.IsEqual(Errors.UnknownPayment.Code, m => m.Code));
        }

        [Fact]
        public async Task Get_ShouldReturn400Error_WhenAccessingPaymentMadeByAnotherMerchant()
        {
            var jobId = (await Post(Data.ValidRequestPaymentCommand, "/payments/", Data.ValidApiKey2))
                .Context.Response.Headers[HeaderNames.Location].First().Replace("/jobs/", "");

            await Task.Delay(TimeSpan.FromSeconds(16));

            var entityId = (await Get<Features.Jobs.Get.Query, Features.Jobs.Get.Model>(new Features.Jobs.Get.Query { JobId = Guid.Parse(jobId) }, "/jobs/{JobId}", Data.ValidApiKey2))
                .Result.EntityId;

            var query = new Get.Query { PaymentId = entityId ?? "" };

            (await Get<ApiError>(query, Data.ValidApiKey))
                .Assert()
                .HttpResponse(a => a.HasStatusCode(HttpStatusCode.BadRequest))
                .Result(r => r.IsEqual(Errors.UnauthorizedAccessToPayment.Code, m => m.Code));
        }
        [Fact]
        public async Task Get_ShouldReturnPayment()
        {
            var jobId = (await Post(Data.ValidRequestPaymentCommand, "/payments/", Data.ValidApiKey))
                .Context.Response.Headers[HeaderNames.Location].First().Replace("/jobs/", "");

            await Task.Delay(TimeSpan.FromSeconds(16));

            var entityId = (await Get<Features.Jobs.Get.Query, Features.Jobs.Get.Model>(new Features.Jobs.Get.Query { JobId = Guid.Parse(jobId) }, "/jobs/{JobId}", Data.ValidApiKey))
                .Result.EntityId;

            var query = new Get.Query { PaymentId = entityId ?? "" };

            (await Get<Get.Model>(query, Data.ValidApiKey))
                .Assert()
                .HttpResponse(a => a.HasStatusCode(HttpStatusCode.OK))
                .Result(r => r
                    .IsEqual(query.PaymentId, m => m.PaymentId)
                    .IsIn(new[] { PaymentStatus.Declined.Value, PaymentStatus.Approved.Value }, m => m.Status)
                    .IsEqual("*************111", m => m.Card.CardNumber)
                    .IsEqual(Data.ValidRequestPaymentCommand.Card.ExpiryMonth, m => m.Card.ExpiryMonth)
                    .IsEqual(Data.ValidRequestPaymentCommand.Card.ExpiryYear, m => m.Card.ExpiryYear)
                    .IsEqual(Data.ValidRequestPaymentCommand.Currency, m => m.Price.Currency)
                    .IsEqual(Data.ValidRequestPaymentCommand.Amount, m => m.Price.Amount)
                    .IsEqual(Data.ValidRequestPaymentCommand.Description, m => m.Description));
        }
    }
}