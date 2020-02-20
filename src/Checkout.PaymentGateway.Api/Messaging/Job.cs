using System;

namespace Checkout.PaymentGateway.Api.Messaging
{
    public enum JobStatus
    {
        Created,
        Failed,
        Completed
    }

    /// <summary>
    /// Asynchronous job.
    /// Calling the SendAsync method of <see cref="IAsyncJobService"/> will automatically create a job so you can track the status.
    /// </summary>
    public class Job
    {
        public Guid JobId { get; private set; }
        public Type UnderlyingCommandType { get; private set; }
        public JobStatus Status { get; private set; }
        public string? EntityId { get; private set; }
        public string? Error { get; private set; }

        /// <summary>
        /// Creates an instance of <see cref="Job"/>
        /// </summary>
        /// <param name="jobId">The JobId determined using the MassTransit MessageId.</param>
        /// <param name="commandType">The CommandType associated to this job.</param>
        public Job(Guid jobId, Type commandType)
        {
            if (jobId == Guid.Empty) throw new ArgumentException($"{nameof(jobId)} cannot be empty.");

            JobId = jobId;
            UnderlyingCommandType = commandType ?? throw new ArgumentNullException(nameof(commandType));
            Status = JobStatus.Created;
        }

        /// <summary>
        /// Mark the job as failed.
        /// </summary>
        /// <param name="error">The message to display while querying jobs.</param>
        public void Fail(string error)
        {
            Error = error ?? throw new ArgumentNullException($"{nameof(error)} cannot be null.");
            EntityId = null;
            Status = JobStatus.Failed;
        }

        /// <summary>
        /// Mark this job as completed.
        /// </summary>
        /// <param name="entityId">The EntityId created by this job.</param>
        public void Complete(string entityId)
        {
            EntityId = entityId ?? throw new ArgumentNullException(nameof(entityId));
            Status = JobStatus.Completed;
        }
    }
}