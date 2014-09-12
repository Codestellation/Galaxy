using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.Tests.Helpers;
using Codestellation.Quarks.IO;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.OperationsTests
{
    [TestFixture]
    public class InstallPackageTests
    {
        private string _feedUri;
        private string _targetPath;

        [SetUp]
        public void Init()
        {
            _feedUri = Path.Combine(Environment.CurrentDirectory, "testnuget");
            Folder.EnsureDeleted(_feedUri);

            EmbeddedResource.ExtractAndRename(_feedUri, "Codestellation.Galaxy.Tests.Resources", "Codestellation.Galaxy.Host.1.0.0", "Codestellation.Galaxy.Host.1.0.0.nupkg");
            EmbeddedResource.ExtractAndRename(_feedUri, "Codestellation.Galaxy.Tests.Resources", "TestNugetPackage.1.0.0", "TestNugetPackage.1.0.0.nupkg");

            _targetPath = Path.Combine(_feedUri, "extracted");
        }

        [Test]
        public void InstallPackage_extract_package_success()
        {
            //given 
            var version10 = new Version(1, 0);
            var order = new[]
            {
                new InstallPackageOrder("TestNugetPackage", _feedUri, version10),
                new InstallPackageOrder("Codestellation.Galaxy.Host", _feedUri, version10),
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
            Folder.EnsureDeleted(_feedUri);
        }
    }
}