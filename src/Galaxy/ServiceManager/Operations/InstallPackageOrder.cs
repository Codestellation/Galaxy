using System;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallPackageOrder
    {
        public readonly string PackageId;
        public readonly string FeedUri;
        public readonly Version Version;

        public InstallPackageOrder(string packageId, string feedUri, Version version)
        {
            PackageId = packageId;
            FeedUri = feedUri;
            Version = version;
        }

        public InstallPackageOrder(string packageId, string feedUri) : this(packageId, feedUri, null)
        {

        }
    }
}