using System;
using System.IO;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.Tests.Content;
using Codestellation.Quarks.IO;
using NUnit.Framework;

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

            TestPackages.CopyHostPackageTo(_nugetFeedFolder);
            TestPackages.CopyTest10To(_nugetFeedFolder);

            _targetPath = Path.Combine(_nugetFeedFolder, "extracted");
        }

        [Test]
        public void InstallPackage_extract_package_success()
        {
            //given
            var version10 = new Version(1, 0);

            var packageDetails = new PackageDetails("TestNugetPackage", _nugetFeedFolder, version10);

            var op = new InstallPackage();
            var stringWriter = new StringWriter();
            var context = new DeploymentTaskContext(stringWriter)
            {
                Folders = new ServiceFolders
                {
                    DeployFolder = (FullPath)_targetPath
                },
                PackageDetails = packageDetails,
                KeepOnUpdate = FileList.Empty
            };

            //when
            op.Execute(context);

            //then
            string[] sampleFiles =
            {
                "TestNugetPackLib.dll",
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