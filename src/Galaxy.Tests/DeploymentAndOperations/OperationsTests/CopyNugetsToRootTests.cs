using System.Text;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.Tests.Helpers;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.OperationsTests
{
    [TestFixture]
    public class CopyNugetsToRootTests
    {
        private string _feedUri;
        private string _targetPath;

        private Deployment _testDeployment;

        [SetUp]
        public void Init()
        {
            _feedUri = Path.Combine(Environment.CurrentDirectory, "testnuget");
            Folder.Delete(_feedUri);
            Folder.Delete(_targetPath);

            EmbeddedResource.ExtractAndRename(_feedUri, "Codestellation.Galaxy.Tests.Resources", "Codestellation.Galaxy.Host.1.0.0", "Codestellation.Galaxy.Host.1.0.0.nupkg");
            EmbeddedResource.ExtractAndRename(_feedUri, "Codestellation.Galaxy.Tests.Resources", "TestNugetPackage.1.0.0", "TestNugetPackage.1.0.0.nupkg");

            
            _testDeployment = new Deployment()
            {
                DisplayName = "testdeployment",
                PackageId = "TestNugetPackage",
                PackageVersion = new Version(1, 0)
            };
            
            _targetPath = Path.Combine(_feedUri, "extracted");

            var version10 = new Version(1, 0);
            var orders = new[]
            {
                new InstallPackage.InstallPackageOrder("TestNugetPackage", _feedUri, version10),
                new InstallPackage.InstallPackageOrder("Codestellation.Galaxy.Host", _feedUri, version10),
            };

            var installPackage = new InstallPackage(_testDeployment.GetDeployFolder(_targetPath), orders);

            var buildLog = new StringWriter();
            installPackage.Execute(buildLog);


            
        }

        [Test]
        public void CopyNugetsToRoot_copy_success()
        {
            string[] sampleFiles = new string[]
            {
                "TestNugetPackLib.dll",
                "Codestellation.Galaxy.Host.exe",
                "Topshelf.dll",
                "Topshelf.NLog.dll"
            };

            var copyNugetsToRoot = new CopyNugetsToRoot(_targetPath, _testDeployment);

            var buildLog = new StringWriter();
            copyNugetsToRoot.Execute(buildLog);

            var files = Directory
                .GetFiles(_targetPath, "*.*", SearchOption.AllDirectories)
                .Select(Path.GetFileName);

            Assert.That(sampleFiles, Is.SubsetOf(files));
        }

        [TearDown]
        public void Cleanup()
        {
            Folder.Delete(_feedUri);
            Folder.Delete(_targetPath);
        }
    }
}