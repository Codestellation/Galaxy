﻿using System.Text;
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
    public class CopyNugetsToRootTests
    {
        private string output;
        private string targetPath;

        private Deployment testDeployment = null;
        private NugetFeed testFeed = null;

        [SetUp]
        public void Init()
        {
            output = Path.Combine(Environment.CurrentDirectory, "testnuget");

            if (Directory.Exists(output))
                Directory.Delete(output, true);

            testFeed = new NugetFeed()
            {
                Name = "TestNugetPackage",
                Uri = output
            };

            testDeployment = new Deployment()
            {
                DisplayName = "testdeployment",
                PackageId = "TestNugetPackage",
                PackageVersion = new Version(1, 0)
            };

            var hostDeployment = new Deployment()
            {
                DisplayName = "testdeployment",
                PackageId = "Codestellation.Galaxy.Host",
                PackageVersion = new Version(1, 0)
            };

            EmbeddedResource.ExtractAndRename(output, "Codestellation.Galaxy.Tests.Resources", "Codestellation.Galaxy.Host.1.0.0", "Codestellation.Galaxy.Host.1.0.0.nupkg");
            EmbeddedResource.ExtractAndRename(output, "Codestellation.Galaxy.Tests.Resources", "TestNugetPackage.1.0.0", "TestNugetPackage.1.0.0.nupkg");

            targetPath = Path.Combine(output, "extracted");

            var op = new InstallPackage(targetPath, testDeployment, testFeed);

            var installHost = new InstallPackage(targetPath, hostDeployment, testFeed);

            var buildLog = new StringBuilder();
            installHost.Execute(buildLog);
            op.Execute(buildLog);
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

            var copyNugetsToRoot = new CopyNugetsToRoot(targetPath, testDeployment, testFeed);

            var buildLog = new StringBuilder();
            copyNugetsToRoot.Execute(buildLog);

            var files = Directory
                .GetFiles(targetPath, "*.*", SearchOption.AllDirectories)
                .Select(Path.GetFileName);

            Assert.That(sampleFiles, Is.SubsetOf(files));
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(output, true);
        }
    }
}