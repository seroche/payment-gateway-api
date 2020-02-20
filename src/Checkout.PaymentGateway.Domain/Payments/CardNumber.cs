using System;
using System.Collections.Generic;
using System.Linq;
using Checkout.PaymentGateway.Domain.Core;
using CreditCardValidator;
using static System.Char;

namespace Checkout.PaymentGateway.Domain.Payments
{
    public class CardNumber : ValueObject
    {
        public string Value { get; private set; }

        private CardNumber(string value) => Value = value;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        /// <summary>
        /// Mask the card number except the specified number of digits starting from the end.
        /// </summary>
        /// <param name="numberOfDigitsShown">The number of digits to show.</param>
        /// <returns>The masked card number.</returns>
        public string Mask(int numberOfDigitsShown)
        {
            if (numberOfDigitsShown <= 0) throw new ArgumentException("Number of digits to show must be greater than 0.");
            return numberOfDigitsShown >= Value.Length
                ? "".PadRight(Value.Length, '*')
                : $"{"".PadRight(Value.Length - numberOfDigitsShown, '*')}{Value.Substring(Value.Length - numberOfDigitsShown, numberOfDigitsShown)}";
        }

        /// <summary>
        /// Returns a <see cref="CardNumber"/>.
        /// </summary>
        /// <param name="number">The card number.</param>
        /// <returns>A <see cref="Result{CardNumber}"/> containing either a card number or an error.</returns>
        public static Result<CardNumber> Create(string number)
        {
            if (number is null) throw new ArgumentNullException(nameof(number));

            var sanitized = Sanitize(number);

            if (string.IsNullOrEmpty(sanitized))
                return Result.Fail<CardNumber>(Errors.InvalidCardNumber);

            var validator = new CreditCardDetector(sanitized);
            return !validator.IsValid()
                ? Result.Fail<CardNumber>(Errors.InvalidCardNumber)
                : Result.Ok(new CardNumber(sanitized));
        }

        private static string Sanitize(string number)
            => new string(number.Where(IsDigit).ToArray());
    }
}