using System;
using System.Collections.Generic;
using Checkout.PaymentGateway.Domain.Core;
using CreditCardValidator;

namespace Checkout.PaymentGateway.Domain.Payments
{
    public class Card : ValueObject
    {
        public CardNumber Number { get; private set; }
        public CardIssuer Type => Number.Value.CreditCardBrand();
        public ExpiryDate ExpiryDate { get; private set; }
        public Cvv Cvv { get; private set; }
        public string? HolderName { get; private set; }

        private Card(CardNumber number, ExpiryDate expiryDate, Cvv cvv, string? holderName)
        {
            Number = number;
            Cvv = cvv;
            ExpiryDate = expiryDate;
            HolderName = holderName;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Number;
        }

        /// <summary>
        /// Returns a <see cref="Card"/>.
        /// </summary>
        /// <param name="number">The <see cref="CardNumber"/>.</param>
        /// <param name="expiryDate">The <see cref="ExpiryDate"/>.</param>
        /// <param name="cvv">The <see cref="Cvv"/>.</param>
        /// <param name="holderName">The card holder name.</param>
        /// <returns>A <see cref="Result{Card}"/> containing either a card or an error.</returns>
        public static Result<Card> Create(
            CardNumber number,
            ExpiryDate expiryDate,
            Cvv cvv,
            string? holderName = null)
        {
            if (number is null) throw new ArgumentNullException(nameof(number));
            if (expiryDate is null) throw new ArgumentNullException(nameof(expiryDate));
            if (cvv is null) throw new ArgumentNullException(nameof(cvv));

            return Result.Ok(new Card(number, expiryDate, cvv, holderName));
        }
    }
}