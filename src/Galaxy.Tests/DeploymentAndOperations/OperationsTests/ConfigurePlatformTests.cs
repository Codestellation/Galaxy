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
        private string _serviceFolder;
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

            var orders = new[] {new InstallPackageOrder( "Codestellation.Galaxy.Host", _nugetFeedUri)};
            _serviceFolder = hostDeployment.GetDeployFolder(OutputFolder);
            var installHost = new InstallPackage(_serviceFolder, orders);

            var buildLog = new StringWriter();

            installHost.Execute(buildLog);

            EmbeddedResource.Extract(Path.Combine(_nugetFeedUri, TestDeployment), "Codestellation.Galaxy.Tests.Resources", "TestNugetPackLib_anycpu.dll");
            EmbeddedResource.Extract(Path.Combine(_nugetFeedUri, TestDeployment), "Codestellation.Galaxy.Tests.Resources", "TestNugetPackLib_x86.dll");
            EmbeddedResource.Extract(Path.Combine(_nugetFeedUri, TestDeployment), "Codestellation.Galaxy.Tests.Resources", "TestNugetPackLib_x64.dll");
        }

        [Test]
        public void Configure_host_platform_to_anycpu_success()
        {
            var assemblyQualifiedType = "TestNugetPackLib.TestServiceClass, TestNugetPackLib_anycpu";
            var platformType = PlatformType.AnyCPU;

            AssertPlatformChanged(assemblyQualifiedType, platformType);
        }

        [Test]
        public void Configure_host_platform_to_x86_success()
        {
            var assemblyQualifiedType = "TestNugetPackLib.TestServiceClass, TestNugetPackLib_x86";
            var platformType = PlatformType.x86;

            AssertPlatformChanged(assemblyQualifiedType, platformType);
        }

        [Test]
        public void Configure_host_platform_to_x64_success()
        {
            var assemblyQualifiedType = "TestNugetPackLib.TestServiceClass, TestNugetPackLib_x64";
            //no need to set target to X64, it works by default
            var platformType = PlatformType.AnyCPU;

            AssertPlatformChanged(assemblyQualifiedType, platformType);
        }

        private void AssertPlatformChanged(string assemblyQualifiedType, PlatformType platformType)
        {
            var hostFileName = "Codestellation.Galaxy.Host.exe";
            var assemblyPath = Path.Combine(_nugetFeedUri, "testdeployment\\Codestellation.Galaxy.Host.exe");

            
            var configurePlatform = new ConfigurePlatform(_serviceFolder, hostFileName, assemblyQualifiedType);

            var buildLog = new StringWriter();
            configurePlatform.Execute(buildLog);
            
            var actualPlatform = PlatformDetector.GetPlatform(assemblyPath);

            Assert.That(actualPlatform, Is.EqualTo(platformType));
        }

        [TearDown]
        public void Cleanup()
        {
            Folder.Delete(_nugetFeedUri);
        }
    }
}