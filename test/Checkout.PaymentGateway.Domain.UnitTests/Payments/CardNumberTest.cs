using Checkout.PaymentGateway.Domain.Payments;
using FluentAssertions;
using System;
using Xunit;

namespace Checkout.PaymentGateway.Domain.UnitTests.Payments
{
    public class CardNumberTest
    {
        [Fact]
        public void Create_ShouldThrowArgumentNullException_WhenCardNumberIsNull()
        {
            Action action = () => CardNumber.Create(null);

            action.Should().Throw<ArgumentNullException>().WithMessage("*number*");
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("1234")]
        [InlineData("0000 1234 1234 1234")]
        [InlineData("ABCD 1234 1234 1234")]
        public void Create_ShouldReturnError_WhenCardNumberIsInvalid(string number)
        {
            var cardNumber = CardNumber.Create(number);

            cardNumber.IsSuccess.Should().BeFalse();
            cardNumber.Error.Should().Be(Errors.InvalidCardNumber);
        }

        [Fact]
        public void Create_ShouldReturnCardNumberWithOnlyDigits()
        {
            var cardNumber = CardNumber.Create("4111 1111 1111 1111");

            cardNumber.IsSuccess.Should().BeTrue();

            var value = cardNumber.GetValue();
            value.Value.Should().Be("4111111111111111");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void Mask_ShouldThrowArgumentException_WhenNumberOfDigitsShownIsLowerOrEqualTo0(int number)
        {
            var cardNumber = CardNumber.Create("4111 1111 1111 1111");
            var value = cardNumber.GetValue();

            Action action = () => value.Mask(number);

            action.Should().Throw<ArgumentException>().WithMessage("Number of digits to show must be greater than 0.");
        }

        [Fact]
        public void Mask_ShouldReturnEntirelyMaskedCardNumber_WhenNumberOfDigitsShownIsGreaterThanCardNumberLength()
        {
            var cardNumber = CardNumber.Create("4111 1111 1111 1111");

            var value = cardNumber.GetValue();
            value.Mask(30).Should().Be("****************");
        }

        [Fact]
        public void Mask_ShouldReturnMaskedCardNumber()
        {
            var cardNumber = CardNumber.Create("4111 1111 1111 1111");

            var value = cardNumber.GetValue();
            value.Mask(3).Should().Be("*************111");
        }
    }
}