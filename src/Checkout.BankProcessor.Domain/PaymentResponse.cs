namespace Checkout.BankProcessor.Domain
{
    public enum PaymentStatus
    {
        Approved,
        Declined
    }

    public class PaymentResponse
    {
        public PaymentStatus Status { get; private set; }
        public string PaymentId { get; private set; }

        public PaymentResponse(PaymentStatus status, string paymentId)
        {
            Status = status;
            PaymentId = paymentId;
        }
    }
}