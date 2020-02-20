using Checkout.PaymentGateway.Domain.Core;

namespace Checkout.PaymentGateway.Domain.Payments.Events
{
    public class PaymentCreated : IEvent
    {
        public string Id { get; private set; }
        
        public PaymentCreated(string id) => Id = id;
    }
}