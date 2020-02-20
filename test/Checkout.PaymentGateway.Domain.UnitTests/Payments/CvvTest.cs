using Checkout.PaymentGateway.Domain.Payments;
using FluentAssertions;
using System;
using Xunit;

namespace Checkout.PaymentGateway.Domain.UnitTests.Payments
{
    public class CvvTest
    {
        [Fact]
        public void Create_ShouldThrowArgumentNullException_WhenCvvIsNull()
        {
            Action action = () => Cvv.Create(null);

            action.Should().Throw<ArgumentNullException>().WithMessage("*code*");
        }

        [Theory]
        [InlineData("12")]
        [InlineData("12345")]
        public void Create_ShouldReturnError_WhenCvvHasAnExpectedLength(string code)
        {
            var cvv = Cvv.Create(code);

            cvv.IsSuccess.Should().BeFalse();
            cvv.Error.Should().Be(Errors.InvalidCvv);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("1234")]
        public void Create_ShouldReturnCvv(string code)
        {
            var cvv = Cvv.Create(code);

            cvv.IsSuccess.Should().BeTrue();

            var value = cvv.GetValue();
            value.Code.Should().Be(code);
        }
    }
}