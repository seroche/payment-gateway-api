using Checkout.PaymentGateway.Domain.Payments;
using Checkout.PaymentGateway.Domain.Payments.Core;
using Checkout.PaymentGateway.Infrastructure.Core;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Infrastructure
{
    /// <summary>
    /// Payment repository. Events are dispatched once the entity is persisted.
    /// </summary>
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IMemoryCache _cache;
        private readonly IEventDispatcher _dispatcher;

        public PaymentRepository(IMemoryCache cache, IEventDispatcher dispatcher)
        {
            _cache = cache;
            _dispatcher = dispatcher;
        }

        public async Task<Payment?> GetAsync(string paymentId, CancellationToken token = default) =>
            await Task.FromResult(!_cache.TryGetValue<Payment>(paymentId, out var payment) ? null : payment);

        public async Task AddAsync(Payment payment, CancellationToken token = default)
        {
            _cache.Set(payment.Id, payment);

            while (payment.Events.TryDequeue(out var e))
                await _dispatcher.PublishAsync(e, token);
        }
    }
}
