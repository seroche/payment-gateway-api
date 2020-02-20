using Checkout.PaymentGateway.Api.Logs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;

namespace Checkout.PaymentGateway.Api
{
    public class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((ctx, services) =>
                {
                    services
                        .AddHttpContextAccessor() // Required to access HttpContext
                        .AddLogging(x => x.ClearProviders());

                    var provider = services.BuildServiceProvider();

                    var logger = new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        // Enrich logs with some custom fields
                        .Enrich.With(new LogEventEnricher(provider.GetRequiredService<IHttpContextAccessor>()))
                        .Destructure.ToMaximumStringLength(1000)
                        .MinimumLevel.Information()
                        .WriteTo.Debug()
                        .CreateLogger();
                    
                    Log.Logger = logger; // Required by RequestLoggingMiddleware
                    var context = new DiagnosticContext(logger);

                    services
                        .AddSingleton(context)
                        .AddSingleton<IDiagnosticContext>(context)
                        .AddSingleton<ILoggerFactory>(new SerilogLoggerFactory(logger, true))
                        .AddSingleton(logger);
                })
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());

    }
}
