using System;

namespace Codestellation.Galaxy.ServiceManager
{
    public class PackageDetails
    {
        public readonly string PackageId;
        public readonly string FeedUri;
        public readonly Version Version;

        public PackageDetails(string packageId, string feedUri, Version version)
        {
            PackageId = packageId;
            FeedUri = feedUri;
            Version = version;
        }

        public PackageDetails(string packageId, string feedUri)
            : this(packageId, feedUri, null)
        {
        }
    }
}