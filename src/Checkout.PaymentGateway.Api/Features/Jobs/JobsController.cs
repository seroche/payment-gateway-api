using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Api.Models;

namespace Checkout.PaymentGateway.Api.Features.Jobs
{
    public class JobsController : CoreController
    {
        private readonly IMediator _mediator;

        public JobsController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Returns a job.
        /// </summary>
        /// <param name="query">The <see cref="Get.Query"/> containing the PaymentId.</param>
        /// <param name="token">The <see cref="CancellationToken"/>.</param>
        [HttpGet("{JobId}")]
        [ProducesResponseType(typeof(ApiError), 404)]
        public async Task<ActionResult<Get.Model>> QueJob([FromRoute] Get.Query query, CancellationToken token)
            => Return(await _mediator.Send(query, token));
    }
}
