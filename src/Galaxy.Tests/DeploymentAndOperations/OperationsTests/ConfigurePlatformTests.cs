using System.Text;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.ServiceManager.Helpers;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.Tests.Helpers;
using NUnit.Framework;
using System;
using System.IO;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.OperationsTests
{
    [TestFixture]
    public class ConfigurePlatformTests
    {
        private string _nugetFeedUri;
        private const string OutputFolder = "output";
        private const string TestDeployment = "testdeployment";

        [SetUp]
        public void Init()
        {
            _nugetFeedUri = Path.Combine(Environment.CurrentDirectory, OutputFolder);

            Folder.Delete(_nugetFeedUri);

            EmbeddedResource.ExtractAndRename(_nugetFeedUri, "Codestellation.Galaxy.Tests.Resources", "Codestellation.Galaxy.Host.1.0.0", "Codestellation.Galaxy.Host.1.0.0.nupkg");

            var hostDeployment = new Deployment()
            {
                DisplayName = TestDeployment,
                PackageId = "Codestellation.Galaxy.Host",
                PackageVersion = new Version(1,0)
            };

            var orders = new[] {new InstallPackage.InstallPackageOrder( "Codestellation.Galaxy.Host", _nugetFeedUri)};
            var installHost = new InstallPackage(hostDeployment.GetDeployFolder(OutputFolder), orders);

            var buildLog = new StringWriter();

            installHost.Execute(buildLog);

            var copyNugetsToRoot = new CopyNugetsToRoot(_nugetFeedUri, hostDeployment);

            copyNugetsToRoot.Execute(buildLog);

            EmbeddedResource.Extract(Path.Combine(_nugetFeedUri, TestDeployment), "Codestellation.Galaxy.Tests.Resources", "TestNugetPackLib_anycpu.dll");
            EmbeddedResource.Extract(Path.Combine(_nugetFeedUri, TestDeployment), "Codestellation.Galaxy.Tests.Resources", "TestNugetPackLib_x86.dll");
            EmbeddedResource.Extract(Path.Combine(_nugetFeedUri, TestDeployment), "Codestellation.Galaxy.Tests.Resources", "TestNugetPackLib_x64.dll");
        }

        [Test]
        public void Configure_host_platform_to_anycpu_success()
        {
            ConfigurePlatform configurePlatform = new ConfigurePlatform(OutputFolder,
                new Deployment()
                {
                    DisplayName = TestDeployment,
                    PackageId = "TestNugetPackage",
                    PackageVersion = new Version(1, 0),
                    AssemblyQualifiedType = "TestNugetPackLib.TestServiceClass, TestNugetPackLib_anycpu"
                });

            var buildLog = new StringWriter();
            configurePlatform.Execute(buildLog);
            Assert.That(PlatformDetector.GetPlatform(Path.Combine(_nugetFeedUri, "testdeployment\\Codestellation.Galaxy.Host.exe")),
                        Is.EqualTo(PlatformType.AnyCPU));
        }

        [Test]
        public void Configure_host_platform_to_x86_success()
        {
            var configurePlatform = new ConfigurePlatform(OutputFolder,
                new Deployment()
                {
                    DisplayName = TestDeployment,
                    PackageId = "TestNugetPackage",
                    PackageVersion = new Version(1, 0),
                    AssemblyQualifiedType = "TestNugetPackLib.TestServiceClass, TestNugetPackLib_x86"
                });
            var buildLog = new StringWriter();
            configurePlatform.Execute(buildLog);

            Assert.That(PlatformDetector.GetPlatform(Path.Combine(_nugetFeedUri, "testdeployment\\Codestellation.Galaxy.Host.exe")),
                        Is.EqualTo(PlatformType.x86));
        }

        [Test]
        public void Configure_host_platform_to_x64_success()
        {
            var configurePlatform = new ConfigurePlatform(OutputFolder,
                new Deployment()
                {
                    DisplayName = TestDeployment,
                    PackageId = "TestNugetPackage",
                    PackageVersion = new Version(1, 0),
                    AssemblyQualifiedType = "TestNugetPackLib.TestServiceClass, TestNugetPackLib_x64"
                });

            var buildLog = new StringWriter();
            configurePlatform.Execute(buildLog);

            Assert.That(PlatformDetector.GetPlatform(Path.Combine(_nugetFeedUri, "testdeployment\\Codestellation.Galaxy.Host.exe")),
                        Is.EqualTo(PlatformType.AnyCPU));
        }

        [TearDown]
        public void Cleanup()
        {
            Folder.Delete(_nugetFeedUri);
        }
    }
}