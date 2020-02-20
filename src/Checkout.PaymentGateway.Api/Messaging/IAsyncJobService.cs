using System;
using System.Threading;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Domain.Core;

namespace Checkout.PaymentGateway.Api.Messaging

{
    /// <summary>
    /// Service used to manage asynchronous job.
    /// </summary>
    public interface IAsyncJobService
    {
        /// <summary>
        /// Starts an asynchronous job.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="token">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        Task<Guid> StartAsync<T>(T command, CancellationToken token = default) where T : ICommand;
        /// <summary>
        /// Returns a job that was previously launched.
        /// If there is no job associated to the given JobId, we return null.
        /// </summary>
        /// <param name="jobId">The JobId.</param>
        /// <param name="token">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        Task<Job?> Get(Guid jobId, CancellationToken token = default);
        /// <summary>
        /// Updates a job. This is usually called to mark a job as completed or failed.
        /// </summary>
        /// <param name="job">The <see cref="Job"/>.</param>
        /// <param name="token">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        Task Update(Job job, CancellationToken token = default);
    }
}