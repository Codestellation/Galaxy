using System;
using System.IO;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Tests.Helpers;
using Codestellation.Galaxy.ServiceManager.Operations;
using NUnit.Framework;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.OperationsTests
{
    [TestFixture]
    public class InstallPackageTests
    {
        string output;
        string targetPath;

        [SetUp]
        public void Init()
        {
            
            output = Path.Combine(Environment.CurrentDirectory, "testnuget");

            ResourcesHelper.ExtractEmbeddedAndRename(output, "Codestellation.Galaxy.Tests.Resources", "TestNugetPackage.1.0.0", "TestNugetPackage.1.0.0.nupkg");

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
                    PackageName = "TestNugetPackage",
                    PackageVersion = new Version(1,0)
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
                    PackageName = "TestNugetPackage"
                },
                new NugetFeed()
                {
                    Name = "TestNugetPackage",
                    Uri = output
                });



            Assert.That(() => op.Execute(),Throws.TypeOf<ArgumentException>() );
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(output, true);
        }
    }
}
