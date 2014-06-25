using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codestellation.Galaxy.Tests.ServiceManager
{
    [TestFixture]
    public class OpetarionsFactoryTests
    {
        public void OperationsFactory_Create_instance()
        {
            OperationsFactory opFac = new OperationsFactory();

            Assert.IsNotNull(opFac);
        }

        [Test]
        public void OperationsFactory_Create_operations()
        {
            OperationsFactory opFac = new OperationsFactory();

            string targetPath = "";
            ServiceApp app = new ServiceApp();
            NugetFeed feed = new NugetFeed();

            var op = opFac.GetCopyNugetsToRootOp(targetPath, app, feed);
            Assert.IsNotNull(op);
            op = opFac.GetInstallPackageOp(targetPath, app, feed);
            Assert.IsNotNull(op);
            op = opFac.GetInstallServiceOp(targetPath, app, feed);
            Assert.IsNotNull(op);
            op = opFac.GetProvideServiceConfigOp(targetPath, app, feed);
            Assert.IsNotNull(op);
            op = opFac.GetStartServiceOp(targetPath, app, feed);
            Assert.IsNotNull(op);
            op = opFac.GetStopServiceOp(targetPath, app, feed);
            Assert.IsNotNull(op);
            op = opFac.GetUninstallPackageOp(targetPath, app, feed);
            Assert.IsNotNull(op);
            op = opFac.GetUninstallServiceOp(targetPath, app, feed);
            Assert.IsNotNull(op);
        }
    }
}
