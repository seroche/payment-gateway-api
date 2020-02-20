using Checkout.PaymentGateway.Api.Messaging;
using Checkout.PaymentGateway.Domain.Core;
using FluentAssertions;
using System;
using Xunit;

namespace Checkout.PaymentGateway.Api.UnitTests.Messaging
{
    public class JobTest
    {
        [Fact]
        public void Ctor_ShouldThrowArgumentException_WhenJobIdIsEmpty()
        {
            Action action = () => new Job(Guid.Empty, typeof(string));

            action.Should().Throw<ArgumentException>().WithMessage("jobId cannot be empty.");
        }

        [Fact]
        public void Ctor_ShouldThrowArgumentNullException_WhenCommandTypeIsNull()
        {
            Action action = () => new Job(Guid.NewGuid(), null);

            action.Should().Throw<ArgumentNullException>().WithMessage("*commandType*");
        }

        [Fact]
        public void Ctor_ShouldInstantiateJob()
        {
            var id = Guid.NewGuid();
            var type = typeof(ICommand);
            var job = new Job(id, type);

            job.JobId.Should().Be(id);
            job.UnderlyingCommandType.Should().Be(type);
            job.Status.Should().Be(JobStatus.Created);
        }

        [Fact]
        public void Complete_ShouldThrowException_WhenEntityIdIsNull()
        {
            var job = new Job(Guid.NewGuid(), typeof(ICommand));
            Action action = () => job.Complete(null);

            action.Should().Throw<ArgumentNullException>().WithMessage("*entityId*");
        }

        [Fact]
        public void Complete_ShouldMarkJobAsCompleted()
        {
            var job = new Job(Guid.NewGuid(), typeof(ICommand));
            var entityId = "123";
            job.Complete(entityId);

            job.EntityId.Should().Be(entityId);
            job.Status.Should().Be(JobStatus.Completed);
        }

        [Fact]
        public void Fail_ShouldThrowException_WhenErrorIsNull()
        {
            var job = new Job(Guid.NewGuid(), typeof(ICommand));
            Action action = () => job.Fail(null);

            action.Should().Throw<ArgumentNullException>().WithMessage("*error*");
        }

        [Fact]
        public void Fail_ShouldMarkJobAsFailed()
        {
            var job = new Job(Guid.NewGuid(), typeof(ICommand));
            var error = "123";
            job.Fail(error);

            job.EntityId.Should().BeNull();
            job.Error.Should().Be(error);
            job.Status.Should().Be(JobStatus.Failed);
        }
    }
}