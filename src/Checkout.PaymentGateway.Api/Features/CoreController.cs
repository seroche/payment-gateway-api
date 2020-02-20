using Checkout.PaymentGateway.Api.Models;
using Checkout.PaymentGateway.Domain.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace Checkout.PaymentGateway.Api.Features
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiError), 500)]
    [ProducesResponseType(typeof(ApiError), 400)]
    [Authorize]
    public class CoreController : ControllerBase
    {
        /// <summary>
        /// Returns a <see cref="JsonResult"/> containing a <see cref="TValue"/> or an <see cref="Error"/>.
        /// </summary>
        /// <typeparam name="TValue">Expected result type</typeparam>
        /// <param name="result">The <see cref="Result{T}"/></param>
        /// <returns></returns>
        [NonAction]
        protected ActionResult<TValue> Return<TValue>(Result<TValue> result)
                where TValue : notnull
        {
            if (result is null) throw new ArgumentNullException(nameof(result));
            return !result.IsSuccess
                ? new JsonResult(new ApiError(result.Error, HttpContext.TraceIdentifier))
                {
                    // Could use a facade to refactor this complexity
                    StatusCode = result.Error switch
                    {
                        UnknownItemError _ => (int)HttpStatusCode.NotFound,
                        _ => (int)HttpStatusCode.BadRequest
                    }
                }
                : new JsonResult(result.GetValue());
        }
    }
}
