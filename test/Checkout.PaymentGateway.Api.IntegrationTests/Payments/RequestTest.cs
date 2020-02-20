using Checkout.PaymentGateway.Api.Features.Payments;
using Checkout.PaymentGateway.Api.IntegrationTests.Core;
using Microsoft.Net.Http.Headers;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Api.Models;
using Xunit;
using Checkout.PaymentGateway.Domain;

namespace Checkout.PaymentGateway.Api.IntegrationTests.Payments
{
    public class RequestTest : ApiTest<Request.Command>
    {
        public RequestTest(WebAppFixture fixture)
            : base(fixture, "/payments")
        { }

        [Theory]
        [InlineData("")]
        [InlineData("test")]
        public async Task Request_ShouldReturn401Error_WhenApiKeyIsMissingOrInvalid(string apiKey)
        {
            var command = new Request.Command();

            (await Post<ApiError>(command, apiKey))
                .Assert()
                .HttpResponse(a => a.HasStatusCode(HttpStatusCode.Unauthorized))
                .Result(r => r.IsEqual(Errors.UnauthorizedAccess.Code, m => m.Code));
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("AAAA")]
        public async Task Request_ShouldReturn400Error_WhenCurrencyIsInvalid(string currency)
        {
            var command = Data.ValidRequestPaymentCommand;
            command.Currency = currency;

            (await Post<ApiError>(command, Data.ValidApiKey))
                .Assert()
                .HttpResponse(a => a.HasStatusCode(HttpStatusCode.BadRequest))
                .Result(r => r.IsEqual(Errors.InvalidCurrency.Code, m => m.Code));
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(0)]
        public async Task Request_ShouldReturn400Error_WhenAmountIsInvalid(decimal amount)
        {
            var command = Data.ValidRequestPaymentCommand;
            command.Amount = amount;
            
            (await Post<ApiError>(command, Data.ValidApiKey))
                .Assert()
                .HttpResponse(a => a.HasStatusCode(HttpStatusCode.BadRequest))
                .Result(r => r.IsEqual(Errors.InvalidAmount.Code, m => m.Code));
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("12")]
        [InlineData("12345")]
        public async Task Request_ShouldReturn400Error_WhenCardNumberIsInvalid(string cardNumber)
        {
            var command = Data.ValidRequestPaymentCommand;
            command.Card.Number = cardNumber;

            (await Post<ApiError>(command, Data.ValidApiKey))
                .Assert()
                .HttpResponse(a => a.HasStatusCode(HttpStatusCode.BadRequest))
                .Result(r => r.IsEqual(Errors.InvalidCardNumber.Code, m => m.Code));
        }

        [Theory]
        [InlineData(-1, 2000)]
        [InlineData(0, 2000)]
        public async Task Request_ShouldReturn400Error_WhenExpiryDateIsInvalid(int month, int year)
        {
            var command = Data.ValidRequestPaymentCommand;
            command.Card.ExpiryMonth = month;
            command.Card.ExpiryYear = year;

            (await Post<ApiError>(command, Data.ValidApiKey))
                .Assert()
                .HttpResponse(a => a.HasStatusCode(HttpStatusCode.BadRequest))
                .Result(r => r.IsEqual(Errors.InvalidExpiryDate.Code, m => m.Code));
        }

        [Fact]
        public async Task Request_ShouldReturn400Error_WhenExpiryDateIsInThePast()
        {
            var command = Data.ValidRequestPaymentCommand;
            command.Card.ExpiryMonth = 1;
            command.Card.ExpiryYear = 2020;

            (await Post<ApiError>(command, Data.ValidApiKey))
                .Assert()
                .HttpResponse(a => a.HasStatusCode(HttpStatusCode.BadRequest))
                .Result(r => r.IsEqual(Errors.ExpiredCard.Code, m => m.Code));
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("12")]
        [InlineData("12345")]
        public async Task Request_ShouldReturn400Error_WhenCvvCodeIsInvalid(string cvv)
        {
            var command = Data.ValidRequestPaymentCommand;
            command.Card.Cvv = cvv;

            (await Post<ApiError>(command, Data.ValidApiKey))
                .Assert()
                .HttpResponse(a => a.HasStatusCode(HttpStatusCode.BadRequest))
                .Result(r => r.IsEqual(Errors.InvalidCvv.Code, m => m.Code));
        }

        [Fact]
        public async Task Request_ShouldReturn202AndLocationHeader_WhenPaymentRequestIsValid()
        {
            var command = Data.ValidRequestPaymentCommand;
            
            (await Post(command, Data.ValidApiKey))
                .Assert()
                .HttpResponse(a => a
                    .HasStatusCode(HttpStatusCode.Accepted)
                    .ContainsHeader(x => x.Key == HeaderNames.Location && x.Value.Any(v => v.StartsWith("/jobs/"))));
        }
    }
}
