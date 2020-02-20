using System;
using Checkout.PaymentGateway.Domain.Core;

namespace Checkout.PaymentGateway.Domain.Payments.Events
{
    public class PaymentDeclined : IEvent
    {
        public string Id { get; private set; }
        public string Reason { get; private set; }

        public PaymentDeclined(string paymentId, string reason)
        {
            Id = paymentId;
            Reason = reason;
        }
    }
}
