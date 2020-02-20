using Checkout.PaymentGateway.Domain.Payments;
using FluentAssertions;
using System;
using Xunit;

namespace Checkout.PaymentGateway.Domain.UnitTests.Payments
{
    public class PriceTest
    {
        [Fact]
        public void Create_ShouldThrowArgumentNullException_WhenCurrencyIsNull()
        {
            Action action = () => Price.Create(null, 10);

            action.Should().Throw<ArgumentNullException>().WithMessage("*currency*");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void Create_ShouldReturnError_WhenAmountIsLowerOrEqualTo0(int amount)
        {
            var price = Price.Create("HKD", amount);

            price.IsSuccess.Should().BeFalse();
            price.Error.Should().Be(Errors.InvalidAmount);
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("AAAA")]
        public void Create_ShouldReturnError_WhenCurrencyIsNot3CharLong(string currency)
        {
            var price = Price.Create(currency, 10);

            price.IsSuccess.Should().BeFalse();
            price.Error.Should().Be(Errors.InvalidCurrency);
        }
    }
}