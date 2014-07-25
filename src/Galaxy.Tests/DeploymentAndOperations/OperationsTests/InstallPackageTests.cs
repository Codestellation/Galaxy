using System.Text;
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
    public class InstallPackageTests
    {
        private string _nugetFeed;
        private string _targetPath;

        [SetUp]
        public void Init()
        {
            _nugetFeed = Path.Combine(Environment.CurrentDirectory, "testnuget");
            Folder.Delete(_nugetFeed);

            EmbeddedResource.ExtractAndRename(_nugetFeed, "Codestellation.Galaxy.Tests.Resources", "TestNugetPackage.1.0.0", "TestNugetPackage.1.0.0.nupkg");

            _targetPath = Path.Combine(_nugetFeed, "extracted");
        }

        [Test]
        public void InstallPackage_extract_package_success()
        {
            var version10 = new Version(1, 0);
            var order = new[] { new InstallPackage.InstallPackageOrder("TestNugetPackage", _nugetFeed, version10)  };
            var op = new InstallPackage(_targetPath, order);
            var buildLog = new StringBuilder();

            op.Execute(buildLog);

            
            var dllFiles = Directory.GetFiles(_targetPath, "*.dll", SearchOption.AllDirectories);

            var expectedDll = dllFiles.FirstOrDefault(item => item.Contains("TestNugetPackLib.dll"));

            Assert.That(expectedDll, Is.Not.Null);
        }

        [TearDown]
        public void Cleanup()
        {
            Folder.Delete(_nugetFeed);
        }
    }
}