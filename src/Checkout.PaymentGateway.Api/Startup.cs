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
using System.IO;
using System.Reflection;
using Microsoft.OpenApi.Models;

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
                .AddSwaggerGen(opts =>
                {
                    opts.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Payment Gateway API",
                        Version = "v1"
                    });
                    opts.CustomSchemaIds(x => x.FullName);
                })
                .AddControllers(opts =>
                {
                    opts.Filters.Add<ModelBindingValidatorFilter>();
                    opts.Filters.Add<ContextLoggingFilter>();
                });
        }

        public void Configure(IApplicationBuilder app) =>
            app
                .UseRouting()
                .UseMiddleware<ErrorHandlingMiddleware>()
                .UseSwagger()
                .UseSwaggerUI(opts =>
                {
                    opts.RoutePrefix = "docs"; 
                    opts.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Gateway API V1");
                })
                .UseSerilogRequestLogging(opts =>
                {
                    // Performance tracker
                    opts.MessageTemplate = "{StatusCode} response returned in {Elapsed:0.0000}ms.";
                    opts.GetLevel = (ctx, dur, ex) => LogEventLevel.Information;
                })
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints => endpoints.MapControllers());
    }
}