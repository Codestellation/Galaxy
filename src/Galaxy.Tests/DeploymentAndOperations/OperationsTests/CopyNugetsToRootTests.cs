﻿using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.Tests.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.OperationsTests
{
    [TestFixture]
    public class CopyNugetsToRootTests
    {
        string output;
        string targetPath;

        Deployment testDeployment = null;

        NugetFeed testFeed = null;

        [SetUp]
        public void Init()
        {
            output = Path.Combine(Environment.CurrentDirectory, "testnuget");

            if(Directory.Exists(output))
                Directory.Delete(output, true);

            testFeed = new NugetFeed()
            {
                Name = "TestNugetPackage",
                Uri = output
            };

            testDeployment = new Deployment()
            {
                DisplayName = "testdeployment",
                PackageName = "TestNugetPackage",
                PackageVersion = new Version(1, 0)
            };

            var hostDeployment = new Deployment()
            {
                DisplayName = "testdeployment",
                PackageName = "Codestellation.Galaxy.Host",
                PackageVersion = new Version(1, 0)
            };

            ResourcesHelper.ExtractEmbeddedAndRename(output, "Codestellation.Galaxy.Tests.Resources", "Codestellation.Galaxy.Host.1.0.0", "Codestellation.Galaxy.Host.1.0.0.nupkg");
            ResourcesHelper.ExtractEmbeddedAndRename(output, "Codestellation.Galaxy.Tests.Resources", "TestNugetPackage.1.0.0", "TestNugetPackage.1.0.0.nupkg");

            targetPath = Path.Combine(output, "extracted");

            InstallPackage op = new InstallPackage(
                targetPath,
                testDeployment,
                testFeed);

            InstallPackage installHost = new InstallPackage(
                targetPath,
                hostDeployment,
                testFeed);

            installHost.Execute();
            op.Execute();
        }

        [Test]
        [Ignore("Depends on tests project appconfig.")]
        public void CopyNugetsToRoot_copy_success()
        {
            string[] sampleFiles = new string[] 
            {
                "TestNugetPackLib.dll",
                "Codestellation.Galaxy.Host.exe",
                "Topshelf.dll",
                "Topshelf.NLog.dll"
            };

            CopyNugetsToRoot copyNugetsToRoot = new CopyNugetsToRoot(targetPath, testDeployment, testFeed);

            copyNugetsToRoot.Execute();
            
            var files = Directory.GetFiles(targetPath, "*.*", SearchOption.AllDirectories).Select(item => Path.GetFileName(item));

            Assert.That(sampleFiles, Is.SubsetOf(files));
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(output, true);
        }
    }
}
