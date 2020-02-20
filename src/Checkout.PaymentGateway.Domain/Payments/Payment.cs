using Checkout.PaymentGateway.Domain.Core;
using System;
using Checkout.PaymentGateway.Domain.Payments.Events;

namespace Checkout.PaymentGateway.Domain.Payments
{
    public class Payment : Entity<string>, IAggregateRoot
    {
        public Guid MerchantId { get; private set; }
        public Card Card { get; private set; }
        public Price Price { get; private set; }
        public string Description { get; private set; }
        public PaymentStatus Status { get; private set; }
        
        private Payment(string paymentId, Guid merchantId, Card card, Price price, string description)
            : base(paymentId)
        {
            MerchantId = merchantId;
            Card = card;
            Price = price;
            Description = description;
            Status = PaymentStatus.Created;
            Events.Enqueue(new PaymentCreated(Id));
        }

        /// <summary>
        /// Approves a payment.
        /// </summary>
        public void Approve()
        {
            Status = PaymentStatus.Approved;
            Events.Enqueue(new PaymentApproved(Id));
        }

        /// <summary>
        /// Declines payment with a reason.
        /// </summary>
        /// <param name="reason"></param>
        public void Decline(string reason)
        {
            Status = PaymentStatus.Declined;
            Events.Enqueue(new PaymentDeclined(Id, reason));
        }

        /// <summary>
        /// Returns a <see cref="Payment"/>.
        /// </summary>
        /// <param name="paymentId">The paymentIds.</param>
        /// <param name="merchantId">The merchantId.</param>
        /// <param name="card">The <see cref="Card"/>.</param>
        /// <param name="price">The <see cref="Price"/>.</param>
        /// <param name="description">The description.</param>
        /// <returns>A <see cref="Payment"/>.</returns>
        public static Payment Create(string paymentId, Guid merchantId, Card card, Price price, string description)
        {
            if (paymentId is null) throw new ArgumentNullException(nameof(paymentId));
            if (merchantId == Guid.Empty) throw new ArgumentException($"{nameof(merchantId)} cannot be empty.");
            if (card is null) throw new ArgumentNullException(nameof(card));
            if (price is null) throw new ArgumentNullException(nameof(price));

            return new Payment(paymentId, merchantId, card, price, description);
        }
    }
}