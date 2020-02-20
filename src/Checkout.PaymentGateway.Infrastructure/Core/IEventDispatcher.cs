using System.Threading;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Domain.Core;

namespace Checkout.PaymentGateway.Infrastructure.Core
{
    /// <summary>
    /// Basic event dispatcher used to publish event raised while interacting with the model
    /// </summary>
    public interface IEventDispatcher
    {
        /// <summary>
        /// Publishes an event.
        /// </summary>
        /// <typeparam name="T">Domain event type</typeparam>
        /// <param name="domainEvent">The <see cref="IEvent"/>.</param>
        /// <param name="token">The <see cref="CancellationToken"/></param>
        Task PublishAsync<T>(T domainEvent, CancellationToken token) 
            where T : IEvent;
    }
}