using System;
using System.IO;
using Alba;
using Baseline;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
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