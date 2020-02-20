using AutoMapper;
using Checkout.PaymentGateway.Api.Mediator;
using Checkout.PaymentGateway.Api.Messaging;
using Checkout.PaymentGateway.Domain;
using Checkout.PaymentGateway.Domain.Core;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Api.Features.Jobs
{
    public class Get
    {
        #region Query
        public class Query : IRequest<Result<Model>>
        {
            public Guid JobId { get; set; }
        }
        #endregion

        #region Model
        public class Model
        {
            public string Status { get; set; }
            public string? Error { get; set; }
            public string? EntityId { get; set; }
        }
        #endregion

        #region Handler
        public class Handler : AsyncHandler<Query, Model>
        {
            private readonly IAsyncJobService _service;
            private readonly IMapper _mapper;

            public Handler(IAsyncJobService service, IMapper mapper)
            {
                _service = service;
                _mapper = mapper;
            }

            public override async Task<Result<Model>> Handle(Query query, CancellationToken token = default)
            {
                var item = await _service.Get(query.JobId, token);
                return item is null
                    ? Fail(Errors.UnknownJob)
                    : Ok(_mapper.Map<Model>(item));
            }
        }
        #endregion
    }
}