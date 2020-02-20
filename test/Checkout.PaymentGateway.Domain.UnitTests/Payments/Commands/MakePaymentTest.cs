using System;
using Checkout.PaymentGateway.Domain.Payments;
using Checkout.PaymentGateway.Domain.Payments.Commands;
using FluentAssertions;
using Xunit;

namespace Checkout.PaymentGateway.Domain.UnitTests.Payments.Commands
{
    public class MakePaymentTest
    {
        [Fact]
        public void Ctor_ShouldThrowArgumentNullException_WhenCardIsNull()
        {
            Action action = () => new MakePayment(Guid.NewGuid(), null, Data.ValidPrice, "test");

            action.Should().Throw<ArgumentNullException>().WithMessage("*card*");
        }

        [Fact]
        public void Ctor_ShouldThrowArgumentNullException_WhenPriceIsNull()
        {
            Action action = () => new MakePayment(Guid.NewGuid(), Data.ValidVisaCard, null, "test");

            action.Should().Throw<ArgumentNullException>().WithMessage("*price*");
        }

        [Fact]
        public void Ctor_ShouldThrowArgumentNullException_WhenDescriptionIsNull()
        {
            Action action = () => new MakePayment(Guid.NewGuid(), Data.ValidVisaCard, Data.ValidPrice, null);

            action.Should().Throw<ArgumentNullException>().WithMessage("*description*");
        }
    }
}