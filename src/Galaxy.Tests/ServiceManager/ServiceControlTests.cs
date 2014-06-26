using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.ServiceManager.Fakes;
using Codestellation.Galaxy.Tests.ServiceManager.Fakes;
using NUnit.Framework;
using System;
using System.Threading;

namespace Codestellation.Galaxy.Tests.ServiceManager
{
    [TestFixture]
    public class ServiceControlTests
    {
        OperationResult ExecuteServiceControl(DeploymentTask task)
        {
            ManualResetEventSlim mre = new ManualResetEventSlim(false);

            OperationResult result = OperationResult.OR_DEFAULT;

            new DeploymentProcessor().Process(task,
                    new EventHandler<Galaxy.ServiceManager.EventParams.DeploymentTaskCompletedEventArgs>(
                        (sender, e) => 
                        {
                            result = e.Result;
                            mre.Set();
                        }));

            mre.Wait();

            return result;
        }

        [Test]
        public void ServiceControl_sequence_success()
        {
            var successSequenceTask = TestTaskBuilder.SequenceTaskSuccess(
                    new Deployment(),
                    new NugetFeed()
                );

            var result = ExecuteServiceControl(successSequenceTask);
            Assert.AreEqual(OperationResult.OR_OK, result);
        }

        [Test]
        public void ServiceControl_sequence_fail()
        {
            var successSequenceTask = TestTaskBuilder.SequenceTaskFail(
                    new Deployment(),
                    new NugetFeed()
                );

            var result = ExecuteServiceControl(successSequenceTask);
            Assert.AreEqual(OperationResult.OR_FAIL, result);
        }  
    }
}
