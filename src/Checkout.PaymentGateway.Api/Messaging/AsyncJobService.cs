using Checkout.PaymentGateway.Domain.Core;
using GreenPipes;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Task = System.Threading.Tasks.Task;

namespace Checkout.PaymentGateway.Api.Messaging
{
    internal class AsyncJobService : IAsyncJobService
    {
        private readonly IBus _bus;
        private readonly IMemoryCache _cache;

        public AsyncJobService(IBus bus, IMemoryCache cache)
        {
            _bus = bus;
            _cache = cache;
        }

        public async Task<Guid> StartAsync<T>(T command, CancellationToken token)
            where T : ICommand
        {
            if (command is null) throw new ArgumentNullException(nameof(command));

            // Pipe is used to retrieve the unique MessageId assigned to our command
            var ctx = new Pipe();
            await _bus.Send(command, ctx, token);

            // Persists the job in memory
            _cache.Set(ctx.MessageId, new Job(ctx.MessageId, typeof(T)));

            return ctx.MessageId;
        }

        public async Task<Job?> Get(Guid jobId, CancellationToken token = default)
            => await Task.FromResult(
                !_cache.TryGetValue<Job>(jobId, out var job) ? null : job);

        public async Task Update(Job job, CancellationToken token = default)
        {
            if (job is null) 
                throw new ArgumentNullException(nameof(job));
            if (!_cache.TryGetValue(job.JobId, out var _))
                throw new InvalidOperationException($"Job '{job.JobId}' cannot be found.");
            
            _cache.Set(job.JobId, job);
            await Task.CompletedTask;
        }
        
        private class Pipe : IPipe<SendContext>
        {
            public Guid MessageId { get; private set; }

            public async Task Send(SendContext context)
            {
                MessageId = context.MessageId ?? Guid.Empty;
                await Task.CompletedTask;
            }

            public void Probe(ProbeContext context) { }
        }
    }
}