using System;
using System.Collections.Generic;
using Checkout.PaymentGateway.Domain.Core;

namespace Checkout.PaymentGateway.Domain.Payments
{
    public class Price : ValueObject
    {
        public string Currency { get; private set; }
        public decimal Amount { get; private set; }
        
        private Price(string currency, decimal amount)
        {
            Currency = currency;
            Amount = amount;
        }

        /// <summary>
        /// Returns a <see cref="Price"/>.
        /// </summary>
        /// <param name="currency">The 3 characters long currency symbol.</param>
        /// <param name="amount">The amount strictly greater than 0.</param>
        /// <returns>A <see cref="Result{Price}"/> containing either a price or an error.</returns>
        public static Result<Price> Create(string currency, decimal amount)
        {
            if (currency is null) throw new ArgumentNullException(nameof(currency));
            if (currency.Length != 3) return Result.Fail<Price>(Errors.InvalidCurrency);
            if (amount <= 0) return Result.Fail<Price>(Errors.InvalidAmount);

            return Result.Ok(new Price(currency, amount));
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Currency;
            yield return Amount;
        }
    }
}