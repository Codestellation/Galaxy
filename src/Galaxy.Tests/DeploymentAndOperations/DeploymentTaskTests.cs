using Codestellation.Emisstar.Testing;
using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.ServiceManager.Events;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes;
using NUnit.Framework;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations
{
    [TestFixture]
    public class DeploymentTaskTests
    {
        private TestPublisher _publisher;
        private TestHandler<DeploymentTaskCompletedEvent> _handler;

        [SetUp]
        public void SetUp()
        {
            _publisher = new TestPublisher();
            _handler = _publisher.RegisterTestHandler<DeploymentTaskCompletedEvent>();
        }

        [Test]
        public void DeploymentProcessor_sequence_success()
        {
            var successSequenceTask = TestTaskBuilder.SequenceTaskSuccess(_publisher);
            var result = Execute(successSequenceTask);
            Assert.AreEqual(ResultCode.Succeed, result.ResultCode);
        }

        [Test]
        public void DeploymentProcessor_sequence_success_task_details()
        {
            var sequence = TestTaskBuilder.SequenceTaskSuccess(_publisher);

            var result = Execute(sequence);

            Assert.That(result.Details, Is.StringContaining(sequence.Name).And.StringContaining("succeed"));
        }

        [Test]
        public void DeploymentProcessor_sequence_fail()
        {
            var sequence = TestTaskBuilder.SequenceTaskFail(_publisher);

            var result = Execute(sequence);
            Assert.AreEqual(ResultCode.Failed, result.ResultCode);
        }

        [Test]
        public void DeploymentProcessor_sequence_fail_task_details()
        {
            var sequence = TestTaskBuilder.SequenceTaskFail(_publisher);

            var result = Execute(sequence);

            Assert.That(result.Details, Is.StringContaining(sequence.Name).And.StringContaining("failed"));
        }

        [Test]
        public void DeploymentProcessor_sequence_fail_task_step_details()
        {
            var sequence = TestTaskBuilder.SequenceTaskFailInTheMiddle(_publisher);

            var result = Execute(sequence);

            Assert.That(result.Details, Is.StringContaining(typeof(FakeOpFail).Name).And.StringContaining("failed"));
        }

        private OperationResult Execute(DeploymentTask task)
        {
            task.Process();
            _handler.WaitUntilCalled(10* 1000);
            return _handler.LastMessage.Result;
        }
    }
}
