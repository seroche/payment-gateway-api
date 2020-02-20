using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Api.Models;
using Checkout.PaymentGateway.Api.Security;

namespace Checkout.PaymentGateway.Api.Features.Payments
{
    public class PaymentsController : CoreController
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
            => _mediator = mediator;

        /// <summary>
        /// Returns a payment.
        /// </summary>
        /// <param name="query">The <see cref="Get.Query"/> containing the PaymentId.</param>
        /// <param name="token">The <see cref="CancellationToken"/>.</param>
        [HttpGet("{PaymentId}")]
        [ProducesResponseType(typeof(ApiError), 404)]
        public async Task<ActionResult<Get.Model>> GetPayment([FromRoute] Get.Query query, CancellationToken token)
        {
            query.MerchantId = Guid.Parse(User.FindFirstValue(CustomClaimTypes.MerchantId));
            return Return(await _mediator.Send(query, token));
        }

        /// <summary>
        /// Requests a payment. Since this operation can take time, this operation is asynchronous.
        /// This endpoint will returns a JobId you will need to use to determine the job status.
        /// </summary>
        /// <param name="command">The <see cref="Request.Command"/> containing all the required information.</param>
        [HttpPost]
        public async Task<ActionResult> RequestPayment([FromBody] Request.Command command)
        {
            command.MerchantId = Guid.Parse(User.FindFirstValue(CustomClaimTypes.MerchantId));
            var res = await _mediator.Send(command);
            return !res.IsSuccess
                ? BadRequest(res.Error)
                : (ActionResult)Accepted($"/jobs/{res.GetValue()}");
        }
    }
}
