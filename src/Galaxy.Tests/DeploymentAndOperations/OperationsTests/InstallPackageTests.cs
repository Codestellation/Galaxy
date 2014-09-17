using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Quarks.IO;
using Codestellation.Quarks.Resources;
using Codestellation.Quarks.Streams;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.OperationsTests
{
    [TestFixture]
    public class InstallPackageTests
    {
        private string _nugetFeedFolder;
        private string _targetPath;

        [SetUp]
        public void Init()
        {
            _nugetFeedFolder = Path.Combine(Environment.CurrentDirectory, "testnuget");
            
            Folder.EnsureDeleted(_nugetFeedFolder);
            Folder.EnsureExists(_nugetFeedFolder);

            var hostPackage = Folder.Combine(_nugetFeedFolder, "Codestellation.Galaxy.Host.1.0.0.nupkg");
            EmbeddedResource
                .EndsWith("Codestellation.Galaxy.Host.1.0.0")
                .ExportTo(hostPackage);

            var testPackage = Folder.Combine(_nugetFeedFolder, "TestNugetPackage.1.0.0.nupkg");
            EmbeddedResource
                .EndsWith("TestNugetPackage.1.0.0")
                .ExportTo(testPackage);

            _targetPath = Path.Combine(_nugetFeedFolder, "extracted");
        }

        [Test]
        public void InstallPackage_extract_package_success()
        {
            //given 
            var version10 = new Version(1, 0);
            var order = new[]
            {
                new InstallPackageOrder("TestNugetPackage", _nugetFeedFolder, version10),
                new InstallPackageOrder("Codestellation.Galaxy.Host", _nugetFeedFolder, version10),
            };
            var op = new InstallPackage(_targetPath, order);
            var buildLog = new StringWriter();

            //when
            op.Execute(buildLog);


            //then
            string[] sampleFiles =
            {
                "TestNugetPackLib.dll",
                "Codestellation.Galaxy.Host.exe",
                "Topshelf.dll",
                "Topshelf.NLog.dll"
            };


            var files = Directory
                .GetFiles(_targetPath)
                .Select(Path.GetFileName);

            Assert.That(sampleFiles, Is.SubsetOf(files));
        }

        [TearDown]
        public void Cleanup()
        {
            Folder.EnsureDeleted(_nugetFeedFolder);
        }
    }
}