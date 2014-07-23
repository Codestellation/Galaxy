using Codestellation.Galaxy.Domain;
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
        private string output;
        private string targetPath;

        [SetUp]
        public void Init()
        {
            output = Path.Combine(Environment.CurrentDirectory, "testnuget");

            EmbeddedResource.ExtractAndRename(output, "Codestellation.Galaxy.Tests.Resources", "TestNugetPackage.1.0.0", "TestNugetPackage.1.0.0.nupkg");

            targetPath = Path.Combine(output, "extracted");
        }

        [Test]
        public void InstallPackage_extract_package_success()
        {
            InstallPackage op = new InstallPackage(
                targetPath,
                new Deployment()
                {
                    DisplayName = "testdeployment",
                    PackageId = "TestNugetPackage",
                    PackageVersion = new Version(1, 0)
                },
                new NugetFeed()
                {
                    Name = "TestNugetPackage",
                    Uri = output
                });

            op.Execute();

            var dllFiles = Directory.GetFiles(targetPath, "*.dll", SearchOption.AllDirectories);

            var expectedDll = dllFiles.FirstOrDefault(item => item.Contains("TestNugetPackLib.dll"));

            Assert.That(expectedDll, Is.Not.Null);
        }

        [Test]
        public void InstallPackage_extract_package_no_version_fail()
        {
            InstallPackage op = new InstallPackage(
                targetPath,
                new Deployment()
                {
                    DisplayName = "testdeployment",
                    PackageId = "TestNugetPackage"
                },
                new NugetFeed()
                {
                    Name = "TestNugetPackage",
                    Uri = output
                });

            Assert.That(() => op.Execute(), Throws.TypeOf<ArgumentException>());
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(output, true);
        }
    }
}