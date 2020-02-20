using Checkout.BankProcessor.Domain;
using Checkout.PaymentGateway.Api.Messaging;
using Checkout.PaymentGateway.Domain.Payments;
using Checkout.PaymentGateway.Domain.Payments.Commands;
using Checkout.PaymentGateway.Domain.Payments.Core;
using MassTransit;
using System.Threading.Tasks;
using PaymentStatus = Checkout.BankProcessor.Domain.PaymentStatus;

namespace Checkout.PaymentGateway.Api.Features.Payments.Consumers
{
    public class MakePaymentConsumer : CommandConsumer<MakePayment>
    {
        private readonly IBankProcessor _processor;
        private readonly IPaymentRepository _repository;

        public MakePaymentConsumer(
            IAsyncJobService service,
            IBankProcessor processor,
            IPaymentRepository repository)
            : base(service)
        {
            _processor = processor;
            _repository = repository;
        }

        public override async Task<string> Execute(ConsumeContext<MakePayment> context)
        {
            var command = context.Message;

            var response = await _processor.ProcessAsync(
                command.Currency,
                command.Amount,
                command.CardNumber,
                command.ExpiryMonth,
                command.ExpiryYear,
                command.Cvv,
                command.CardHolderName);

            // We assume command sent through the bus are ALWAYS valid. In other words, the code below should never fail.
            var payment = Payment.Create(
                response.PaymentId,
                command.MerchantId,
                Card.Create(
                    CardNumber.Create(command.CardNumber).GetValue(),
                    ExpiryDate.Create(command.ExpiryMonth, command.ExpiryYear).GetValue(),
                    Cvv.Create(command.Cvv).GetValue(),
                    command.CardHolderName).GetValue(),
                Price.Create(command.Currency, command.Amount).GetValue(),
                command.Description);

            // This is simplistic :)
            if (response.Status == PaymentStatus.Approved)
                payment.Approve();
            else
                payment.Decline("Payment was declined");

            // Persist the payment
            await _repository.AddAsync(payment);

            // We return the Id of the created entity!
            return response.PaymentId;
        }
    }
}
