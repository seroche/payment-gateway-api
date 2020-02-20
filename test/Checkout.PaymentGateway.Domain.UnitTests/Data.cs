using Checkout.PaymentGateway.Domain.Payments;
using System;

namespace Checkout.PaymentGateway.Domain.UnitTests
{
    public static class Data
    {
        public static CardNumber ValidVisaCardNumber
            => CardNumber.Create("4111 1111 1111 1111").GetValue();

        public static ExpiryDate ValidExpiryDate
            => ExpiryDate.Create(1, 2099).GetValue();

        public static Cvv ValidCvv
            => Cvv.Create("123").GetValue();

        public static Card ValidVisaCard
            => Card.Create(ValidVisaCardNumber, ValidExpiryDate, ValidCvv, "Sebastien").GetValue();

        public static Price ValidPrice
            => Price.Create("HKD", 100).GetValue();

        public static Payment ValidPayment
            => Payment.Create("123", Guid.NewGuid(), ValidVisaCard, ValidPrice, "Description");
    }
}
