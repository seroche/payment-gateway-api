using System;
using Checkout.PaymentGateway.Domain.Core;
using FluentAssertions;
using Xunit;

namespace Checkout.PaymentGateway.Domain.UnitTests.Core
{
    public class ResultTest
    {
        [Fact]
        public void Fail_ShouldThrowArgumentNullException_WhenErrorIsNull()
        {
            Action action = () => Result.Fail<string>(null);

            action.Should().Throw<ArgumentNullException>().WithMessage("*error*");
        }

        [Fact]
        public void Fail_ShouldReturnFailure()
        {
            var expected = new Error("NO");
            var result = Result.Fail<string>(expected);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(expected);
        }

        [Fact]
        public void Ok_ShouldReturnSuccess()
        {
            var expected = "test";
            var result = Result.Ok(expected);

            result.IsSuccess.Should().BeTrue();
            result.GetValue().Should().Be(expected);
        }

        [Fact]
        public void GetValue_ShouldThrowInvalidOperationException_WhenIsSuccessIsFalse()
        {
            Action action = () => Result.Fail<string>(new Error("ETE")).GetValue();

            action.Should().Throw<InvalidOperationException>().WithMessage("Value cannot be retrieved in case of failure.");
        }
    }
}