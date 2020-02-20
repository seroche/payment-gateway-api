using AutoMapper;
using Checkout.BankProcessor.Domain;
using Checkout.BankProcessor.SimpleOne;
using Checkout.PaymentGateway.Api.Logs;
using Checkout.PaymentGateway.Api.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using System.Reflection;

namespace Checkout.PaymentGateway.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetAssembly(typeof(Startup)) ?? throw new NotSupportedException();

            services
                // To return consistent errors, disable this filter so we can use ours!
                .Configure<ApiBehaviorOptions>(opts => opts.SuppressModelStateInvalidFilter = true)
                .AddMemoryCache() // For this test, we persist data in memory to keep things simple.
                .AddSecurity()
                .AddAutoMapper(assembly)
                .AddPipeline(assembly)
                .AddBus(assembly)
                .AddPersistence()
                .AddScoped<IBankProcessor, SimpleOneBankProcessor>()
                .AddControllers(opts =>
                {
                    opts.Filters.Add<ModelBindingValidatorFilter>();
                    opts.Filters.Add<ContextLoggingFilter>();
                });
        }

        public void Configure(IApplicationBuilder app) =>
            app
                .UseRouting()
                .UseSerilogRequestLogging(opts =>
                {
                    // Performance tracker
                    opts.MessageTemplate = "{StatusCode} response returned in {Elapsed:0.0000}ms.";
                    opts.GetLevel = (ctx, dur, ex) => LogEventLevel.Information;
                })
                .UseMiddleware<ErrorHandlingMiddleware>()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints => endpoints.MapControllers());
    }
}