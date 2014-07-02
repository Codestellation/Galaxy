using NuGet;
using System;
using System.Linq;
using System.Collections.Generic;
using Codestellation.Galaxy.Domain;
using System.Collections.Concurrent;
using System.Threading;
using NLog;

namespace Codestellation.Galaxy.Infrastructure
{

    public class VersionPackageCache
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// < packageID, Version[] >
        /// </summary>
        private readonly ConcurrentDictionary<string, HashSet<SemanticVersion>> _packageVersionCache =
            new ConcurrentDictionary<string, HashSet<SemanticVersion>>();

        private readonly ConcurrentDictionary<string, NugetFeed> _packageFeeds =
            new ConcurrentDictionary<string, NugetFeed>();

        private readonly Lazy<Timer> refreshTimerLazy;
        private readonly TimeSpan refreshInterval = TimeSpan.FromMinutes(5);
        private readonly TimeSpan startupRefreshDelay = TimeSpan.FromSeconds(10);

        public VersionPackageCache()
        {
            var callback = new TimerCallback((param) => RefreshCache());
            refreshTimerLazy = new Lazy<Timer>(() => new Timer(callback, null, startupRefreshDelay, refreshInterval));
        }

        private void CacheVersion(string packageID, SemanticVersion version)
        {
            var packageVersions = _packageVersionCache[packageID];
            packageVersions.Add(version);
        }

        private void RefreshCacheForPackage(string packageID)
        {
            var feed = _packageFeeds[packageID];

            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(feed.Uri);

            var packages = repo.FindPackagesById(packageID);

            foreach (var packageItem in packages)
            {
                CacheVersion(packageID, packageItem.Version);
            }
        }

        private void RefreshCache()
        {
            var packages = _packageVersionCache.Keys.ToArray();

            foreach (var package in packages)
            {
                RefreshCacheForPackage(package);
            }

            Log.Info("Nuget packages versions cache successfully refreshed.");

            CallOnCacheUpdated();
        }

        private void CallOnCacheUpdated()
        {
            var eventHander = OnCacheUpdated;
            if (eventHander != null)
                eventHander(this, new EventArgs());
        }

        public IEnumerable<Version> GetPackageVersions(string packageID)
        {
            if (_packageVersionCache.ContainsKey(packageID))
                return _packageVersionCache[packageID].Select(item => item.Version);
            else
                return new Version[0];
        }

        public void AddPackage(string packageID, NugetFeed feed)
        {
            _packageVersionCache.TryAdd(packageID, new HashSet<SemanticVersion>());
            _packageFeeds.TryAdd(packageID, feed);

            refreshTimerLazy.Value.Change(startupRefreshDelay, refreshInterval);            
        }

        public event EventHandler OnCacheUpdated;
    }
}
