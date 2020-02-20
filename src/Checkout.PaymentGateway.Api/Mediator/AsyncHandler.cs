using System.Threading;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Domain.Core;
using MediatR;

namespace Checkout.PaymentGateway.Api.Mediator
{
    /// <summary>
    /// Handler accepting a <see cref="IRequest{TResponse}"/> and returning a <see cref="Result{T}"/>.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    public abstract class AsyncHandler<TRequest, TResult> : IRequestHandler<TRequest, Result<TResult>>
        where TResult : notnull
        where TRequest : IRequest<Result<TResult>>
    {
        /// <summary>
        /// Returns a failure
        /// </summary>
        /// <param name="error">The <see cref="Error"/>.</param>
        /// <returns></returns>
        protected Result<TResult> Fail(Error error) => Result.Fail<TResult>(error);
        /// <summary>
        /// Returns a success
        /// </summary>
        /// <param name="result">The expected object.</param>
        /// <returns></returns>
        protected Result<TResult> Ok(TResult result) => Result.Ok(result);

        public abstract Task<Result<TResult>> Handle(TRequest command, CancellationToken token = default);
    }
}