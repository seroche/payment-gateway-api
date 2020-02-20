using System.Collections.Generic;
using Checkout.PaymentGateway.Domain.Core;

namespace Checkout.PaymentGateway.Domain.Payments
{
    public class PaymentStatus : Enumeration
    {
        public string Value { get; private set; }
     
        /// <summary>
        /// Payment created
        /// </summary>
        public static PaymentStatus Created => new PaymentStatus(nameof(Created));
        /// <summary>
        /// Payment approved by the bank
        /// </summary>
        public static PaymentStatus Approved => new PaymentStatus(nameof(Approved));
        /// <summary>
        /// Payment declined by the bank
        /// </summary>
        public static PaymentStatus Declined => new PaymentStatus(nameof(Declined));

        private PaymentStatus(string value)
            => Value = value;
        
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}
