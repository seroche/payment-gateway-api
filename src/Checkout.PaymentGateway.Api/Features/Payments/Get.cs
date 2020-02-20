using AutoMapper;
using Checkout.PaymentGateway.Api.Mediator;
using Checkout.PaymentGateway.Domain;
using Checkout.PaymentGateway.Domain.Core;
using Checkout.PaymentGateway.Domain.Payments.Core;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Api.Features.Payments
{
    public class Get
    {
        #region Query
        public class Query : IRequest<Result<Model>>
        {
            internal Guid MerchantId { get; set; }
            public string PaymentId { get; set; }
        }
        #endregion

        #region Model
        public class Model
        {
            public string PaymentId { get; set; }
            public string Status { get; set; }
            public CardModel Card { get; set; }
            public PriceModel Price { get; set; }
            public string Description { get; set; }

            public class CardModel
            {
                public string CardNumber { get; set; }
                public int ExpiryMonth { get; set; }
                public int ExpiryYear { get; set; }
            }

            public class PriceModel
            {
                public string Currency { get; set; }
                public decimal Amount { get; set; }
            }
        }
        #endregion

        #region Handler
        public class Handler : AsyncHandler<Query, Model>
        {
            private readonly IPaymentRepository _repository;
            private readonly IMapper _mapper;

            public Handler(IPaymentRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public override async Task<Result<Model>> Handle(Query query, CancellationToken token = default)
            {
                var request = await _repository.GetAsync(query.PaymentId, token);

                if (request is null)
                    return Fail(Errors.UnknownPayment);
                if (request.MerchantId != query.MerchantId)
                    return Fail(Errors.UnauthorizedAccessToPayment);

                return Ok(_mapper.Map<Model>(request));
            }
        }
        #endregion
    }
}