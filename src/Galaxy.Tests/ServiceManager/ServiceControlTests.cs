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
        IOperationsFactory startFailFactory = null;

        [SetUp]
        public void Init()
        {
            StubIOperationsFactory stub = new StubIOperationsFactory();

            stub.GetStartServiceOpStringDeploymentNugetFeed =
                new Microsoft.QualityTools.Testing.Fakes.FakesDelegates.Func<string, Deployment, NugetFeed, ServiceOperation>(
                    (path, deployment, feed) => new FakeOpFail(path, deployment, feed));

            startFailFactory = stub;
        }

        [Test]
        public void ServiceControl_Create_instance()
        {
            ServiceControl sc = new ServiceControl(new FakeOpFactory(), new Deployment(), new NugetFeed());

            Assert.IsNotNull(sc);
        }

        OperationResult ExecuteServiceControl(Action<ServiceControl> action, IOperationsFactory opFactory = null)
        {
            ManualResetEventSlim mre = new ManualResetEventSlim(false);

            var opFactoryLocal = opFactory == null ? new FakeOpFactory() : opFactory;

            ServiceControl sc = new ServiceControl(opFactoryLocal, new Deployment(), new NugetFeed());

            action(sc);            

            OperationResult result = OperationResult.OR_DEFAULT;

            sc.OnCompleted += new System.EventHandler<Galaxy.ServiceManager.EventParams.OperationCompletedEventArgs>((sender, e) =>
            {
                result = e.Result;
                mre.Set();
            });
            sc.Operate();
            mre.Wait();

            return result;
        }

        [Test]
        public void ServiceControl_install_success()
        {
            var result = ExecuteServiceControl(sc => sc.AddInstall());
            Assert.AreEqual(OperationResult.OR_OK, result);
        }
        
        [Test]
        public void ServiceControl_start_success()
        {
            var result = ExecuteServiceControl(sc => sc.AddStart());
            Assert.AreEqual(OperationResult.OR_OK, result);
        }

        [Test]
        public void ServiceControl_start_fail()
        {
            var result = ExecuteServiceControl(sc => sc.AddStart(), startFailFactory);
            Assert.AreEqual(OperationResult.OR_FAIL, result);
        }

        [Test]
        public void ServiceControl_stop_success()
        {
            var result = ExecuteServiceControl(sc => sc.AddStop());
            Assert.AreEqual(OperationResult.OR_OK, result);
        }

        [Test]
        public void ServiceControl_uninstall_success()
        {
            var result = ExecuteServiceControl(sc => sc.AddUninstall());
            Assert.AreEqual(OperationResult.OR_OK, result);           
        }
    }
}
