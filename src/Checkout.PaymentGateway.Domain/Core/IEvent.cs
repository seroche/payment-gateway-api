using MediatR;

namespace Checkout.PaymentGateway.Domain.Core
{
    // Marker used to identify events.
    public interface IEvent : INotification
    { }
}