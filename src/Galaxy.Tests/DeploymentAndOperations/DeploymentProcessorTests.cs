using System;
using System.Threading;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.ServiceManager.Fakes;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.Tests.DeploymentAndOperations.Fakes;
using NUnit.Framework;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations
{
    [TestFixture]
    public class DeploymentProcessorTests
    {
        OperationResult ExecuteServiceControl(DeploymentTask task)
        {
            ManualResetEventSlim deploymentCompleted = new ManualResetEventSlim(false);

            OperationResult result = null;

            new DeploymentProcessor().Process(task,
                    new EventHandler<Galaxy.ServiceManager.EventParams.DeploymentTaskCompletedEventArgs>(
                        (sender, e) => 
                        {
                            result = e.Result;
                            deploymentCompleted.Set();
                        }));

            deploymentCompleted.Wait();

            return result;
        }

        [Test]
        public void DeploymentProcessor_sequence_success()
        {
            var successSequenceTask = TestTaskBuilder.SequenceTaskSuccess(
                    new Deployment(),
                    new NugetFeed()
                );

            var result = ExecuteServiceControl(successSequenceTask);
            Assert.AreEqual(OperationResultType.OR_OK, result.ResultType);
        }

        [Test]
        public void DeploymentProcessor_sequence_success_task_details()
        {
            var sequence = TestTaskBuilder.SequenceTaskSuccess(
                    new Deployment(),
                    new NugetFeed()
                );

            var result = ExecuteServiceControl(sequence);
            Assert.IsTrue(result.Details.Contains(sequence.Name) &&
                          result.Details.Contains("succeeded"));
        }

        [Test]
        public void DeploymentProcessor_sequence_fail()
        {
            var sequence = TestTaskBuilder.SequenceTaskFail(
                    new Deployment(),
                    new NugetFeed()
                );

            var result = ExecuteServiceControl(sequence);
            Assert.AreEqual(OperationResultType.OR_FAIL, result.ResultType);
        }

        [Test]
        public void DeploymentProcessor_sequence_fail_task_details()
        {
            var sequence = TestTaskBuilder.SequenceTaskFail(
                    new Deployment(),
                    new NugetFeed()
                );

            var result = ExecuteServiceControl(sequence);
            Assert.IsTrue(result.Details.Contains(sequence.Name) &&
                          result.Details.Contains("failed"));
        }

        [Test]
        public void DeploymentProcessor_sequence_fail_task_step_details()
        {
            var sequence = TestTaskBuilder.SequenceTaskFailInTheMiddle(
                    new Deployment(),
                    new NugetFeed()
                );

            var result = ExecuteServiceControl(sequence);
            Assert.IsTrue(result.Details.Contains(typeof(FakeOpFail).Name) &&
                          result.Details.Contains("failed"));
        } 
    }
}
