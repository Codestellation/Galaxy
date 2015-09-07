using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace Codestellation.Galaxy.Infrastructure
{
    public class PackageBoard
    {
        public IEnumerable<Version> GetPackageVersions(string source, string packageId)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                return new Version[0];
            }

            var repo = PackageRepositoryFactory.Default.CreateRepository(source);

            var versions = repo
                .FindPackagesById(packageId)
                .Select(x => x.Version.Version)
                .ToArray();
            return versions;
        }
    }
}