using Checkout.PaymentGateway.Api.Models;
using Checkout.PaymentGateway.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Api.Security
{
    [SuppressMessage("Microsoft.Performance", "CA1812")]
    internal class ModelBindingValidatorFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new JsonResult(
                    new ApiError(
                        Errors.MissingRequiredParameter,
                        context.HttpContext.TraceIdentifier))
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
                return;
            }
            await next();
        }
    }
}
