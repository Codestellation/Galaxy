using Codestellation.Galaxy.Domain;
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
        private string outputPath;
        private const string OutputFolder = "output";
        private const string TestDeployment = "testdeployment";

        [SetUp]
        public void Init()
        {
            outputPath = Path.Combine(Environment.CurrentDirectory, OutputFolder);

            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, true);

            EmbeddedResource.ExtractAndRename(outputPath, "Codestellation.Galaxy.Tests.Resources", "Codestellation.Galaxy.Host.1.0.0", "Codestellation.Galaxy.Host.1.0.0.nupkg");

            var hostDeployment = new Deployment()
            {
                DisplayName = TestDeployment,
                PackageId = "Codestellation.Galaxy.Host",
                PackageVersion = new Version(1, 0)
            };

            var testFeed = new NugetFeed()
            {
                Name = "TestNugetPackage",
                Uri = outputPath
            };

            InstallPackage installHost = new InstallPackage(
                outputPath,
                hostDeployment,
                testFeed);

            installHost.Execute();

            CopyNugetsToRoot copyNugetsToRoot = new CopyNugetsToRoot(outputPath, hostDeployment, testFeed);

            copyNugetsToRoot.Execute();

            EmbeddedResource.Extract(Path.Combine(outputPath, TestDeployment), "Codestellation.Galaxy.Tests.Resources", "TestNugetPackLib_anycpu.dll");
            EmbeddedResource.Extract(Path.Combine(outputPath, TestDeployment), "Codestellation.Galaxy.Tests.Resources", "TestNugetPackLib_x86.dll");
            EmbeddedResource.Extract(Path.Combine(outputPath, TestDeployment), "Codestellation.Galaxy.Tests.Resources", "TestNugetPackLib_x64.dll");
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
                },
                new NugetFeed()
                {
                    Name = "TestNugetPackage",
                    Uri = ""
                });

            configurePlatform.Execute();
            Assert.That(PlatformDetector.GetPlatform(Path.Combine(outputPath, "testdeployment\\Codestellation.Galaxy.Host.exe")),
                        Is.EqualTo(PlatformType.AnyCPU));
        }

        [Test]
        public void Configure_host_platform_to_x86_success()
        {
            ConfigurePlatform configurePlatform = new ConfigurePlatform(OutputFolder,
                new Deployment()
                {
                    DisplayName = TestDeployment,
                    PackageId = "TestNugetPackage",
                    PackageVersion = new Version(1, 0),
                    AssemblyQualifiedType = "TestNugetPackLib.TestServiceClass, TestNugetPackLib_x86"
                },
                new NugetFeed()
                {
                    Name = "TestNugetPackage",
                    Uri = ""
                });

            configurePlatform.Execute();

            Assert.That(PlatformDetector.GetPlatform(Path.Combine(outputPath, "testdeployment\\Codestellation.Galaxy.Host.exe")),
                        Is.EqualTo(PlatformType.x86));
        }

        [Test]
        public void Configure_host_platform_to_x64_success()
        {
            ConfigurePlatform configurePlatform = new ConfigurePlatform(OutputFolder,
                new Deployment()
                {
                    DisplayName = TestDeployment,
                    PackageId = "TestNugetPackage",
                    PackageVersion = new Version(1, 0),
                    AssemblyQualifiedType = "TestNugetPackLib.TestServiceClass, TestNugetPackLib_x64"
                },
                new NugetFeed()
                {
                    Name = "TestNugetPackage",
                    Uri = ""
                });

            configurePlatform.Execute();

            Assert.That(PlatformDetector.GetPlatform(Path.Combine(outputPath, "testdeployment\\Codestellation.Galaxy.Host.exe")),
                        Is.EqualTo(PlatformType.AnyCPU));
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(outputPath, true);
        }
    }
}