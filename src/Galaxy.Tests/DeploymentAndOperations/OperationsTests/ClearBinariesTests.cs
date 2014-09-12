using System;
using System.IO;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Galaxy.Tests.Helpers;
using Codestellation.Quarks.IO;
using NUnit.Framework;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.OperationsTests
{
    public class ClearBinariesTests
    {
        private string _nugetFeedFolder;
        private string _basePath;

        private StringWriter _buildLog;

        [SetUp]
        public void Init()
        {
            _nugetFeedFolder = Path.Combine(Environment.CurrentDirectory, "testnuget");

            Folder.EnsureDeleted(_nugetFeedFolder);

            var version10 = new Version(1, 0);

            EmbeddedResource.ExtractAndRename(_nugetFeedFolder, "Codestellation.Galaxy.Tests.Resources", "Codestellation.Galaxy.Host.1.0.0", "Codestellation.Galaxy.Host.1.0.0.nupkg");
            EmbeddedResource.ExtractAndRename(_nugetFeedFolder, "Codestellation.Galaxy.Tests.Resources", "TestNugetPackage.1.0.0", "TestNugetPackage.1.0.0.nupkg");

            _basePath = Path.Combine(_nugetFeedFolder, "extracted");

            var orders = new[]
            {
                new InstallPackageOrder("TestNugetPackage", _nugetFeedFolder, version10),
                new InstallPackageOrder("Codestellation.Galaxy.Host", _nugetFeedFolder, version10),
            };

            var installPackage = new InstallPackage(_basePath, orders, FileList.Empty);


            _buildLog = new StringWriter();
            installPackage.Execute(_buildLog);
            
        }

        [Test]
        public void Deletes_files_exclude_specified_file()
        {
            //given
            var fileToSkip = "Codestellation.Galaxy.Host.exe.config";

            var keepOnUpdate = new FileList(new[] { fileToSkip });

            var operation = new ClearBinaries(_basePath, keepOnUpdate);

            //when
            operation.Execute(_buildLog);

            //then
            var file = Directory
                .GetFiles(_basePath, "*.*", SearchOption.AllDirectories)
                .Select(Path.GetFileName)
                .SingleOrDefault();

            Assert.That(file, Is.StringEnding(fileToSkip));
        }


        [Test]
        public void Deletes_folders_exclude_specified_folder()
        {
            //given
            var folderToSkip = "database";
            var aFileInFolder = "read.me";

            var folderPath = Path.Combine(_basePath, folderToSkip);
            Directory.CreateDirectory(folderPath);


            var filePath = Path.Combine(folderPath, aFileInFolder);

            File.WriteAllText(filePath,"Please, please! Do not delete me!");

            var keepOnUpdate = new FileList(new[] { folderToSkip });

            var operation = new ClearBinaries(_basePath, keepOnUpdate);

            //when
            operation.Execute(_buildLog);

            //then
            Assert.That(Directory.Exists(folderPath), Is.True, "Directory was removed");
            Assert.That(File.Exists(filePath), Is.True, "File in directory was removed");
        }

        [TearDown]
        public void Cleanup()
        {
            Folder.EnsureDeleted(_nugetFeedFolder);
        }
    }
}