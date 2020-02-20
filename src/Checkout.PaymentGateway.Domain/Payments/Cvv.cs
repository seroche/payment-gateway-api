using System;
using System.Collections.Generic;
using System.Linq;
using Checkout.PaymentGateway.Domain.Core;

namespace Checkout.PaymentGateway.Domain.Payments
{
    public class Cvv : ValueObject
    {
        public static int[] AllowedCvvLength = { 3, 4 };

        private Cvv(string code) => Code = code;

        public string Code { get; private set; }
        
        /// <summary>
        /// Returns a <see cref="Cvv"/>.
        /// </summary>
        /// <param name="code">The CVV code.</param>
        /// <returns>A <see cref="Cvv"/> containing either a CVV code or an error.</returns>
        public static Result<Cvv> Create(string code)
        {
            if (code is null) throw new ArgumentNullException(nameof(code));

            return !AllowedCvvLength.Contains(code.Length)
                ? Result.Fail<Cvv>(Errors.InvalidCvv)
                : Result.Ok(new Cvv(code));
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Code;
        }
    }
}
