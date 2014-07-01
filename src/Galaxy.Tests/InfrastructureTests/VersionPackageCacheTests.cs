using Codestellation.Galaxy.Infrastructure;
using Codestellation.Galaxy.Tests.Helpers;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace Codestellation.Galaxy.Tests.InfrastructureTests
{
    [TestFixture]
    public class VersionPackageCacheTests
    {
        const string testPackageName = "TestNugetPackage";

        string output;

        [SetUp]
        public void Init()
        {
            output = Path.Combine(Environment.CurrentDirectory, "testnuget");

            ResourcesHelper.ExtractEmbeddedAndRename(output, "Codestellation.Galaxy.Tests.Resources", "TestNugetPackage.1.0.0", "TestNugetPackage.1.0.0.nupkg");
            ResourcesHelper.ExtractEmbeddedAndRename(output, "Codestellation.Galaxy.Tests.Resources", "TestNugetPackage.1.1.0", "TestNugetPackage.1.1.0.nupkg");
        }

        [Test]
        public void VersionPackageCache_retrieve_package_versions_success()
        {
            ManualResetEventSlim refreshCompleted = new ManualResetEventSlim(false);

            VersionPackageCache versionPackageCache = new VersionPackageCache();
            versionPackageCache.OnCacheUpdated += new EventHandler((sender, e) => refreshCompleted.Set());

            versionPackageCache.AddPackage(testPackageName, new Domain.NugetFeed() 
            { 
                Name = "test_feed",
                Uri = output
            });

            refreshCompleted.Wait();

            var packageVersions = versionPackageCache.GetPackageVersions(testPackageName);

            Version[] sampleVesrions = new Version[]
            {
                new Version(1,1,0,0),
                new Version(1,0,0,0)
            };

            Assert.That(packageVersions, Is.EquivalentTo(sampleVesrions));
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(output, true);
        }
    }
}
