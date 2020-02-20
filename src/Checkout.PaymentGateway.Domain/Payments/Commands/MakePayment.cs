using Checkout.PaymentGateway.Domain.Core;
using System;

namespace Checkout.PaymentGateway.Domain.Payments.Commands
{
    public class MakePayment : ICommand
    {
        public Guid MerchantId { get; private set; }
        public string Currency { get; private set; }
        public decimal Amount { get; private set; }
        public string CardNumber { get; private set; }
        public int ExpiryMonth { get; private set; }
        public int ExpiryYear { get; private set; }
        public string Cvv { get; private set; }
        public string? CardHolderName { get; private set; }
        public string Description { get; private set; }

        protected MakePayment() { }

        public MakePayment(Guid merchantId, Card card, Price price, string description)
        {
            if (card is null) throw new ArgumentNullException(nameof(card));
            if (price is null) throw new ArgumentNullException(nameof(price));
            if (description is null) throw new ArgumentNullException(nameof(description));

            MerchantId = merchantId;
            Currency = price.Currency;
            Amount = price.Amount;
            CardNumber = card.Number.Value;
            ExpiryMonth = card.ExpiryDate.Month;
            ExpiryYear = card.ExpiryDate.Year;
            Cvv = card.Cvv.Code;
            CardHolderName = card.HolderName;
            Description = description;
        }
    }
}