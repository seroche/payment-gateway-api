using MassTransit;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Api.Messaging
{
    /// <summary>
    /// BusService used internally by MassTransit to start/stop the bus
    /// </summary>
    internal class BusService : IHostedService
    {
        private readonly IBusControl _busControl;

        public BusService(IBusControl busControl) => _busControl = busControl;
        public Task StartAsync(CancellationToken token) => _busControl.StartAsync(token);
        public Task StopAsync(CancellationToken token) => _busControl.StopAsync(token);
    }
}