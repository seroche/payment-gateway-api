using System;
using Checkout.PaymentGateway.Domain.Core;

namespace Checkout.PaymentGateway.Domain.Payments.Events
{
    public class PaymentApproved : IEvent
    {
        public string Id { get; private set; }

        public PaymentApproved(string paymentId) 
            => Id = paymentId;
    }
}
