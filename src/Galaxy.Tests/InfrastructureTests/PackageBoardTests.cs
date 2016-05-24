using System;
using System.IO;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.Tests.Content;
using Codestellation.Quarks.IO;
using NUnit.Framework;

namespace Codestellation.Galaxy.Tests.InfrastructureTests
{
    [TestFixture]
    public class PackageBoardTests
    {
        private const string TestPackageId = "TestNugetPackage";

        private string _nugetFeedFolder;

        [SetUp]
        public void CreateFeed()
        {
            _nugetFeedFolder = Path.Combine(Environment.CurrentDirectory, "testnuget");

            Folder.EnsureDeleted(_nugetFeedFolder);
            Folder.EnsureExists(_nugetFeedFolder);

            TestPackages.CopyTest10To(_nugetFeedFolder);
            TestPackages.CopyTest11To(_nugetFeedFolder);
        }

        [Test]
        public void VersionPackageCache_retrieve_package_versions_success()
        {
            //given
            var versionCache = new PackageBoard();
            //when
            var packageVersions = versionCache.GetPackageVersions(_nugetFeedFolder, TestPackageId);
            //then
            var sampleVersions = new[] { new Version(1, 1, 0, 0), new Version(1, 0, 0, 0) };
            Assert.That(packageVersions, Is.EquivalentTo(sampleVersions));
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(_nugetFeedFolder, true);
        }
    }
}