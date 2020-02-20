using Checkout.PaymentGateway.Domain.Payments;
using Checkout.PaymentGateway.Domain.Payments.Events;
using FluentAssertions;
using System;
using Xunit;

namespace Checkout.PaymentGateway.Domain.UnitTests.Payments
{
    public class PaymentTest
    {
        [Fact]
        public void Create_ShouldThrowArgumentNullException_WhenPaymentIdIsNull()
        {
            Action action = () => Payment.Create(null, Guid.NewGuid(), Data.ValidVisaCard, Data.ValidPrice, "My Description");

            action.Should().Throw<ArgumentNullException>().WithMessage("*paymentId*");
        }

        [Fact]
        public void Create_ShouldThrowArgumentException_WhenMerchantIdIsEmpty()
        {
            Action action = () => Payment.Create("test", Guid.Empty, Data.ValidVisaCard, Data.ValidPrice, "My Description");

            action.Should().Throw<ArgumentException>().WithMessage("merchantId cannot be empty.");
        }

        [Fact]
        public void Create_ShouldThrowArgumentNullException_WhenCardIsNull()
        {
            Action action = () => Payment.Create("test", Guid.NewGuid(), null, Data.ValidPrice, "My Description");

            action.Should().Throw<ArgumentNullException>().WithMessage("*card*");
        }

        [Fact]
        public void Create_ShouldThrowArgumentNullException_WhenPriceIsNull()
        {
            Action action = () => Payment.Create("test", Guid.NewGuid(), Data.ValidVisaCard, null, "My Description");

            action.Should().Throw<ArgumentNullException>().WithMessage("*price*");
        }

        [Fact]
        public void Create_ShouldReturnPayment()
        {
            var expectedPaymentId = "test";
            var expectedMerchantId = Guid.NewGuid();
            var expectedDescription = "description";
            var payment = Payment.Create(expectedPaymentId, expectedMerchantId, Data.ValidVisaCard, Data.ValidPrice, expectedDescription);

            payment.Id.Should().Be(expectedPaymentId);
            payment.MerchantId.Should().Be(expectedMerchantId);
            payment.Status.Should().Be(PaymentStatus.Created);
            payment.Card.Should().Be(Data.ValidVisaCard);
            payment.Price.Should().Be(Data.ValidPrice);
            payment.Description.Should().Be(expectedDescription);
            payment.Events.Should().Contain(x => x is PaymentCreated && ((PaymentCreated)x).Id == expectedPaymentId);
        }

        [Fact]
        public void Approve_ShouldMarkPaymentAsApproved()
        {
            var payment = Data.ValidPayment;

            payment.Approve();

            payment.Status.Should().Be(PaymentStatus.Approved);
            payment.Events.Should().Contain(x => x is PaymentApproved && ((PaymentApproved)x).Id == payment.Id);
        }

        [Fact]
        public void Decline_ShouldMarkPaymentAsDeclined()
        {
            var payment = Data.ValidPayment;
            var expectedReason = "Nope";

            payment.Decline(expectedReason);

            payment.Status.Should().Be(PaymentStatus.Declined);
            payment.Events.Should().Contain(x => x is PaymentDeclined && ((PaymentDeclined)x).Id == payment.Id && ((PaymentDeclined)x).Reason == expectedReason);
        }
    }
}