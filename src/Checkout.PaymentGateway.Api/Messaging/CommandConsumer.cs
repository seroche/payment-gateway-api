using Checkout.PaymentGateway.Domain.Core;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Api.Messaging
{
    /// <summary>
    /// Base Command Consumer created to abstract the complexity related to job creation and execution
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CommandConsumer<T> : IConsumer<T>
        where T : class, ICommand
    {
        private readonly IAsyncJobService _service;

        protected CommandConsumer(IAsyncJobService service) => _service = service;

        public async Task Consume(ConsumeContext<T> context)
        {
            var jobId = context.MessageId ?? throw new InvalidOperationException("MessageId cannot be extracted from context.");
            var job = await _service.Get(jobId) ?? throw new InvalidOperationException($"Job '{jobId}' cannot be found.");

            try
            {
                var entityId = await Execute(context);

                if (string.IsNullOrEmpty(entityId))
                    job.Fail("EntityId returned by consumer is either null or empty.");
                else
                    job.Complete(entityId);
            }
            catch (Exception e)
            {
                job.Fail(e.Message);
            }

            await _service.Update(job);
        }

        /// <summary>
        /// Executes a command and returns a string containing the created entity's id.
        /// </summary>
        /// <param name="context">The <see cref="ConsumeContext{T}"/>.</param>
        /// <returns>The Entity Id</returns>
        public abstract Task<string> Execute(ConsumeContext<T> context);
    }
}