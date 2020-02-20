using System;
using System.Threading.Tasks;
using Checkout.BankProcessor.Domain;

namespace Checkout.BankProcessor.SimpleOne
{
    public class SimpleOneBankProcessor : IBankProcessor
    {
        public async Task<PaymentResponse> ProcessAsync(
            string currency, 
            decimal amount, 
            string cardNumber, 
            int expiryMonth, 
            int expiryYear, 
            string cvv,
            string? cardHolderName)
        {
            await Task.Delay(TimeSpan.FromSeconds(15));

            var paymentId = Guid.NewGuid().ToString();

            return await Task.FromResult(amount < 1000
                ? new PaymentResponse(PaymentStatus.Approved, paymentId)
                : new PaymentResponse(PaymentStatus.Declined, paymentId));
        }
    }
}