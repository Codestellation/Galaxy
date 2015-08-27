using System;
using System.IO;
using System.Threading;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Domain.Deployments;
using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.Tests.Content;
using Codestellation.Pulsar.Schedulers;
using Codestellation.Quarks.IO;
using NUnit.Framework;

namespace Codestellation.Galaxy.Tests.InfrastructureTests
{
    [TestFixture]
    public class PackageVersionCacheTests
    {
        private const string TestPackageId = "TestNugetPackage";

        private string _nugetFeedFolder;
        private Repository _repository;

        [SetUp]
        public void Init()
        {
            _repository = new Repository();
            _repository.Start();

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
            var refreshCompleted = new ManualResetEventSlim(false);

            var feedBoard = new FeedBoard();

            var deploymentBoard = new DeploymentBoard(_repository, new Options());
            var nugetFeed = new NugetFeed() { Uri = _nugetFeedFolder };

            feedBoard.AddFeed(nugetFeed);
            deploymentBoard.AddDeployment(new Deployment { FeedId = nugetFeed.Id, PackageId = TestPackageId });
            var scheduler = new PulsarScheduler();

            var versionCache = new PackageVersionBoard(feedBoard, deploymentBoard, scheduler);

            //when
            versionCache.ForceRefresh();
            versionCache.Refreshed += refreshCompleted.Set;
            Assert.That(refreshCompleted.Wait(TimeSpan.FromSeconds(20)), Is.True, "Cache update timeout");

            //then
            var packageVersions = versionCache.GetPackageVersions(nugetFeed.Id, TestPackageId);
            var sampleVesrions = new[] { new Version(1, 1, 0, 0), new Version(1, 0, 0, 0) };
            Assert.That(packageVersions, Is.EquivalentTo(sampleVesrions));
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(_nugetFeedFolder, true);
            _repository.Dispose();
        }
    }
}