using System.Threading.Tasks;

namespace Checkout.BankProcessor.Domain
{
    public interface IBankProcessor
    {
        /// <summary>
        /// Processes a payment.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="expiryMonth">The expiry month.</param>
        /// <param name="expiryYear">The expiry year.</param>
        /// <param name="cvv">The CVV code.</param>
        /// <param name="cardHolderName">The card holder name.</param>
        /// <returns>A <see cref="PaymentResponse"/> containing the payment status and its id.</returns>
        Task<PaymentResponse> ProcessAsync(string currency, decimal amount, string cardNumber, int expiryMonth, int expiryYear, string cvv, string? cardHolderName);
    }
}
