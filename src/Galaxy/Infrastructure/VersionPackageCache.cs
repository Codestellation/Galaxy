﻿using NuGet;
using System;
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
        private readonly ConcurrentDictionary<NugetFeed, HashSet<SemanticVersion>> _packageVersionCache =
            new ConcurrentDictionary<NugetFeed, HashSet<SemanticVersion>>();

        Timer refreshTimer = null;
        readonly TimeSpan refreshInterval = TimeSpan.FromMinutes(5);
        readonly TimeSpan startupRefreshDelay = TimeSpan.FromSeconds(10);

        private void CacheVersion(NugetFeed package, SemanticVersion version)
        {
            var packageVersions = _packageVersionCache[package];
            packageVersions.Add(version);
        }

        private void RefreshCacheForPackage(NugetFeed package)
        {
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(package.Uri);

            var packages = repo.FindPackagesById(package.Name);

            foreach (var packageItem in packages)
            {
                CacheVersion(package, packageItem.Version);
            }
        }

        private void RefreshCache()
        {
            var packages = _packageVersionCache.Keys;
            foreach (var package in packages)
            {
                RefreshCacheForPackage(package);
            }

            Log.Info("Nuget packages versions cache successfully refreshed.");
        }

        public IEnumerable<SemanticVersion> GetPackageVersions(NugetFeed package)
        {
            if (_packageVersionCache.ContainsKey(package))
                return _packageVersionCache[package];
            else
                return new SemanticVersion[0];
        }

        public void AddPackage(NugetFeed feed)
        {
            _packageVersionCache.TryAdd(feed, new HashSet<SemanticVersion>());

            if (refreshTimer == null)
                refreshTimer = new Timer(
                    new TimerCallback(
                        (param) => RefreshCache()), null, startupRefreshDelay, refreshInterval);

            refreshTimer.Change(startupRefreshDelay, refreshInterval);            
        }
    }
}
