using Checkout.PaymentGateway.Domain.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Infrastructure.Core
{
    public class AuRevoirEventDispatcher : IEventDispatcher
    {
        public Task PublishAsync<T>(T command, CancellationToken token) where T : IEvent
            => Task.CompletedTask;
    }
}
