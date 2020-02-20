using Checkout.PaymentGateway.Api.Mediator;
using Checkout.PaymentGateway.Api.Messaging;
using Checkout.PaymentGateway.Domain.Core;
using Checkout.PaymentGateway.Domain.Payments;
using Checkout.PaymentGateway.Domain.Payments.Commands;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Api.Features.Payments
{
    public class Request
    {
        #region Command
        public class Command : IRequest<Result<Guid>>
        {
            internal Guid MerchantId { get; set; } // Should be retrieved from the claims
            public string Currency { get; set; }
            public decimal Amount { get; set; }
            public string? Description { get; set; }
            public CardDetails Card { get; set; }

            public class CardDetails
            {
                public string Number { get; set; }
                public string Cvv { get; set; }
                public int ExpiryMonth { get; set; }
                public int ExpiryYear { get; set; }
                public string HolderName { get; set; }
            }
        }
        #endregion

        #region Validator
        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => Price.Create(x.Currency, x.Amount))
                    .Must(x => x.IsSuccess)
                    .WithState((x, res) => res.Error);

                RuleFor(x => CardNumber.Create(x.Card.Number))
                    .Must(x => x.IsSuccess)
                    .WithState((x, res) => res.Error);

                RuleFor(x => ExpiryDate.Create(x.Card.ExpiryMonth, x.Card.ExpiryYear))
                    .Must(x => x.IsSuccess)
                    .WithState((x, res) => res.Error);

                RuleFor(x => Cvv.Create(x.Card.Cvv))
                    .Must(x => x.IsSuccess)
                    .WithState((x, res) => res.Error);
            }
        }
        #endregion

        #region Handler
        public class Handler : AsyncHandler<Command, Guid>
        {
            private readonly IAsyncJobService _bus;

            public Handler(IAsyncJobService bus) => _bus = bus;

            public override async Task<Result<Guid>> Handle(Command command, CancellationToken token = default)
            {
                // This cannot fail since we validated the entire command before
                var price = Price.Create(command.Currency, command.Amount).GetValue();
                var number = CardNumber.Create(command.Card.Number).GetValue();
                var expiryDate = ExpiryDate.Create(command.Card.ExpiryMonth, command.Card.ExpiryYear).GetValue();
                var cvv = Cvv.Create(command.Card.Cvv).GetValue();

                var card = Card.Create(number, expiryDate, cvv, command.Card.HolderName);

                // Starts the async job
                return Ok(
                    await _bus.StartAsync(
                        new MakePayment(command.MerchantId, card.GetValue(), price, command.Description ?? string.Empty), token));
            }
        }
        #endregion
    }
}