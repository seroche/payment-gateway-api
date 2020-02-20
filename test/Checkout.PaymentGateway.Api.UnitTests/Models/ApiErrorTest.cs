using Checkout.PaymentGateway.Api.Models;
using Checkout.PaymentGateway.Domain.Core;
using FluentAssertions;
using Xunit;

namespace Checkout.PaymentGateway.Api.UnitTests.Models
{
    public class ApiErrorTest
    {
        [Fact]
        public void Ctor_ShouldInstantiateApiErrorAndSetCodeToEmptyIfErrorIsNull()
        {
            var requestId = "123";
            var apiError = new ApiError(null, requestId);

            apiError.Code.Should().BeEmpty();
            apiError.RequestId.Should().Be(requestId);
        }

        [Fact]
        public void Ctor_ShouldInstantiateApiError()
        {
            var error = new Error("test");
            var requestId = "123";
            var apiError = new ApiError(new Error("test"), requestId);

            apiError.Code.Should().Be(error.Code);
            apiError.RequestId.Should().Be(requestId);
        }
    }
}
