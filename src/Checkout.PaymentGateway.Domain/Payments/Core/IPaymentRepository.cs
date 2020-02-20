using System.Threading;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Domain.Payments.Core
{
    public interface IPaymentRepository
    {
        /// <summary>
        /// Gets a payment using its id.
        /// </summary>
        /// <param name="paymentId">The paymentId.</param>
        /// <param name="token">THe <see cref="CancellationToken"/>.</param>
        /// <returns>The payment or null if the payment cannot be found.</returns>
        public Task<Payment?> GetAsync(string paymentId, CancellationToken token = default);
        /// <summary>
        /// Adds a payment.
        /// </summary>
        /// <param name="payment">The <see cref="Payment"/>.</param>
        /// <param name="token">The <see cref="CancellationToken"/>.</param>
        public Task AddAsync(Payment payment, CancellationToken token = default);
    }
}