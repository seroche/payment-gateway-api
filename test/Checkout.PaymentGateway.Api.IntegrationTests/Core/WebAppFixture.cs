using System;
using System.IO;
using Alba;
using Baseline;
using Checkout.PaymentGateway.Api.Logs;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;
using Xunit;

namespace Checkout.PaymentGateway.Api.IntegrationTests.Core
{

    [CollectionDefinition(nameof(WebAppCollection))]
    public class WebAppCollection : ICollectionFixture<WebAppFixture> { }

    public sealed class WebAppFixture : IDisposable
    {
        public SystemUnderTest SystemUnderTest { get; }

        public WebAppFixture()
        {
            var contentRoot = typeof(Startup).Assembly.Location;
            var builder = WebHost
                .CreateDefaultBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(((ctx, services) =>
                {
                    var logger = new LoggerConfiguration()
                        .WriteTo.Debug()
                        .CreateLogger();

                    Log.Logger = logger; // Required by RequestLoggingMiddleware
                    var context = new DiagnosticContext(logger);

                    services
                        .AddSingleton(context)
                        .AddSingleton<IDiagnosticContext>(context)
                        .AddSingleton<ILoggerFactory>(new SerilogLoggerFactory(logger, true))
                        .AddSingleton(logger);
                }))
                .UseEnvironment("Development")
                .UseContentRoot(
                    Path.Combine(
                        contentRoot.Substring(0, contentRoot.IndexOf("test", StringComparison.OrdinalIgnoreCase)),
                        "src\\Checkout.PaymentGateway.Api"));

            SystemUnderTest = new SystemUnderTest(builder);
        }

        public void Dispose()
            => SystemUnderTest.SafeDispose();
    }
}