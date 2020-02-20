using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Domain.Core;
using FluentValidation;
using MediatR;
using static System.Reflection.BindingFlags;

namespace Checkout.PaymentGateway.Api.Mediator.Behaviors
{
    [SuppressMessage("Microsoft.Performance", "CA1812")]
    internal class ServiceProviderValidatorFactory : ValidatorFactoryBase
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderValidatorFactory(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public override IValidator CreateInstance(Type validatorType)
            => (IValidator)_serviceProvider.GetService(validatorType);
    }

    /// <summary>
    /// Behavior validating all queries and commands before sending them to a handler.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    internal class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<Result>
    {
        private readonly IValidatorFactory _factory;
        private readonly List<Type> _types = new List<Type>();

        public ValidationBehavior(IValidatorFactory factory)
        {
            _factory = factory;
            _types.Add(typeof(TRequest));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken token, RequestHandlerDelegate<TResponse> next)
        {
            var validators = _types
                .Select(t => _factory.GetValidator(t))
                .Where(validator => validator is { });

            foreach (var validator in validators)
            {
                var validation = await validator.ValidateAsync(request, token);
                if (validation.IsValid) continue;

                if (validation.Errors.First().CustomState is Error err)
                {
                    // We don't need Object[] here. Cast to `Object` will result in constructor of `Result` not found.
                    // ReSharper disable once CoVariantArrayConversion
                    var res = Activator.CreateInstance(typeof(TResponse), NonPublic | Instance, null, new[] { err }, null);
                    if (res is { }) return (TResponse)res;
                }
                throw new NotSupportedException($"Validation custom state must be of a non-null '{nameof(Error)}'.");
            }

            return await next();
        }
    }
}