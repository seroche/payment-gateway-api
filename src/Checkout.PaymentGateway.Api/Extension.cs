using Checkout.PaymentGateway.Api.Features.Payments.Consumers;
using Checkout.PaymentGateway.Api.Mediator.Behaviors;
using Checkout.PaymentGateway.Api.Messaging;
using Checkout.PaymentGateway.Api.Security;
using Checkout.PaymentGateway.Domain.Payments.Commands;
using Checkout.PaymentGateway.Domain.Payments.Core;
using Checkout.PaymentGateway.Infrastructure;
using Checkout.PaymentGateway.Infrastructure.Core;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Checkout.PaymentGateway.Api
{
    public static class Extension
    {
        /// <summary>
        /// Register the security layer.
        /// In our case, we will rely on <see cref="AuthenticationHandler{TOptions}"/> to perform the authentication.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns></returns>
        public static IServiceCollection AddSecurity(this IServiceCollection services)
            => services
                .AddScoped<IApiKeyService, ApiKeyService>()
                .AddAuthentication(ApiKeyAuthenticationSchemeOptions.Scheme)
                .AddScheme<ApiKeyAuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationSchemeOptions.Scheme, cfg => { })
                .Services;

        /// <summary>
        /// Register the persistence layer.
        /// In our case, we will rely on <see cref="IMemoryCache"/> to persist data.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPersistence(this IServiceCollection services) =>
            services.AddScoped<IPaymentRepository, PaymentRepository>();

        /// <summary>
        /// Registers our pipeline system built with mediatr.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies">The list of <see cref="Assembly"/> to go through to list all available <see cref="AbstractValidator{T}"/>.</param>
        /// <returns></returns>
        public static IServiceCollection AddPipeline(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            services.AddMediatR(assemblies);

            AssemblyScanner
                .FindValidatorsInAssemblies(assemblies)
                .ForEach(scan => services
                    .AddTransient(scan.InterfaceType, scan.ValidatorType)
                    .AddTransient(scan.ValidatorType, scan.ValidatorType));

            return services
                .AddTransient<IValidatorFactory, ServiceProviderValidatorFactory>()
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        /// <summary>
        /// Registers our message system built on top of MassTransit.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies">The list of <see cref="Assembly"/> to go through to list all available <see cref="IConsumer{TMessage}"/></param>
        /// <returns></returns>
        public static IServiceCollection AddBus(this IServiceCollection services, params Assembly[] assemblies) =>
            services
                .AddSingleton<IHostedService, BusService>()
                .AddScoped<IAsyncJobService, AsyncJobService>()
                .AddScoped<IEventDispatcher, AuRevoirEventDispatcher>()
                .AddMassTransit(opts =>
                {
                    opts.AddInMemoryBus((provider, cfg) =>
                    {
                        // In this case we one single receive endpoint.
                        cfg.ReceiveEndpoint("make-payment", ep =>
                        {
                            ep.ConfigureConsumer(provider, typeof(MakePaymentConsumer));
                            EndpointConvention.Map<MakePayment>(ep.InputAddress);
                        });
                    });
                    opts.AddConsumers(assemblies);
                });
    }
}