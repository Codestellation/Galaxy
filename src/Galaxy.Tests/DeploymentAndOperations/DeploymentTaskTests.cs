using System;
using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.ServiceManager.Events;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes;
using MediatR;
using NSubstitute;
using NUnit.Framework;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations
{
    [TestFixture]
    public class DeploymentTaskTests
    {
        private IMediator _mediator;
        private DeploymentTask _task;

        [SetUp]
        public void SetUp()
        {
            _mediator = Substitute.For<IMediator>();
        }

        [Test]
        public void DeploymentProcessor_sequence_success()
        {
            _task = TestTaskBuilder.SequenceTaskSuccess(_mediator);
            Assert(e => e.Result.ResultCode == ResultCode.Succeed);
        }

        [Test]
        public void DeploymentProcessor_sequence_success_task_details()
        {
            _task = TestTaskBuilder.SequenceTaskSuccess(_mediator);

            Assert(e => e.Result.Details.Contains(_task.Name) && e.Result.Details.Contains("succeed"));
        }

        [Test]
        public void DeploymentProcessor_sequence_fail()
        {
            _task = TestTaskBuilder.SequenceTaskFail(_mediator);

            Assert(e => e.Result.ResultCode == ResultCode.Failed);
        }

        [Test]
        public void DeploymentProcessor_sequence_fail_task_details()
        {
            _task = TestTaskBuilder.SequenceTaskFail(_mediator);


            Assert(e => e.Result.Details.Contains(_task.Name) && e.Result.Details.Contains("failed"));

        }

        [Test]
        public void DeploymentProcessor_sequence_fail_task_step_details()
        {
            _task = TestTaskBuilder.SequenceTaskFailInTheMiddle(_mediator);

            Assert(e => e.Result.Details.Contains(typeof(FakeOpFail).Name) && e.Result.Details.Contains("failed"));
        }

        private void Assert(Predicate<DeploymentTaskCompletedEvent> matching)
        {
            _task.Process();
            _mediator.Received(1).Send(Arg.Is<DeploymentTaskCompletedEvent>(x => matching(x)));
        }
    }
}