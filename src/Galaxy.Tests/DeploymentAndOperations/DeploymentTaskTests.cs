using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes;
using NUnit.Framework;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations
{
    [TestFixture]
    public class DeploymentTaskTests
    {
        [Test]
        public void DeploymentProcessor_sequence_success()
        {
            var successSequenceTask = TestTaskBuilder.SequenceTaskSuccess();
            var result = ExecuteServiceControl(successSequenceTask);
            Assert.AreEqual(ResultCode.Succeed, result.ResultCode);
        }

        [Test]
        public void DeploymentProcessor_sequence_success_task_details()
        {
            var sequence = TestTaskBuilder.SequenceTaskSuccess();

            var result = ExecuteServiceControl(sequence);

            Assert.That(result.Details, Is.StringContaining(sequence.Name).And.StringContaining("succeed"));
        }

        [Test]
        public void DeploymentProcessor_sequence_fail()
        {
            var sequence = TestTaskBuilder.SequenceTaskFail();

            var result = ExecuteServiceControl(sequence);
            Assert.AreEqual(ResultCode.Failed, result.ResultCode);
        }

        [Test]
        public void DeploymentProcessor_sequence_fail_task_details()
        {
            var sequence = TestTaskBuilder.SequenceTaskFail();

            var result = ExecuteServiceControl(sequence);

            Assert.That(result.Details, Is.StringContaining(sequence.Name).And.StringContaining("failed"));
        }

        [Test]
        public void DeploymentProcessor_sequence_fail_task_step_details()
        {
            var sequence = TestTaskBuilder.SequenceTaskFailInTheMiddle();

            var result = ExecuteServiceControl(sequence);

            Assert.That(result.Details, Is.StringContaining(typeof(FakeOpFail).Name).And.StringContaining("failed"));
        }

        private OperationResult ExecuteServiceControl(DeploymentTask task)
        {
            OperationResult result = null;

            task.Process(e => { result = e.Result; });

            task.Wait();

            return result;
        }
    }
}
