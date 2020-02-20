using Checkout.PaymentGateway.Domain.Payments;
using CreditCardValidator;
using FluentAssertions;
using System;
using Xunit;

namespace Checkout.PaymentGateway.Domain.UnitTests.Payments
{
    public class CardTest
    {
        [Fact]
        public void Create_ShouldThrowArgumentNullException_WhenCardNumberIsNull()
        {
            Action action = () => Card.Create(null, Data.ValidExpiryDate, Data.ValidCvv);

            action.Should().Throw<ArgumentNullException>().WithMessage("*number*");
        }

        [Fact]
        public void Create_ShouldThrowArgumentNullException_WhenExpiryDateIsNull()
        {
            Action action = () => Card.Create(Data.ValidVisaCardNumber, null, Data.ValidCvv);

            action.Should().Throw<ArgumentNullException>().WithMessage("*expiryDate*");
        }

        [Fact]
        public void Create_ShouldThrowArgumentNullException_WhenCvvIsNull()
        {
            Action action = () => Card.Create(Data.ValidVisaCardNumber, Data.ValidExpiryDate, null);

            action.Should().Throw<ArgumentNullException>().WithMessage("*cvv*");
        }

        [Fact]
        public void Create_ShouldReturnCard()
        {
            var card = Card.Create(Data.ValidVisaCardNumber, Data.ValidExpiryDate, Data.ValidCvv);

            card.IsSuccess.Should().BeTrue();

            var value = card.GetValue();
            value.Number.Should().Be(Data.ValidVisaCardNumber);
            value.ExpiryDate.Should().Be(Data.ValidExpiryDate);
            value.Cvv.Should().Be(Data.ValidCvv);
            value.Type.Should().Be(CardIssuer.Visa);
        }
    }
}