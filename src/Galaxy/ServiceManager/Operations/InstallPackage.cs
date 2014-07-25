using System;
using System.IO;
using System.Text;
using NuGet;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallPackage: IOperation
    {
        private readonly string _destination;
        private readonly InstallPackageOrder[] _orders;

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
        
        public InstallPackage(string destination, InstallPackageOrder[] orders)
        {
            _destination = destination;
            _orders = orders;
        }

        public void Execute(TextWriter buildLog)
        {
            foreach (var order in _orders)
            {
                Install(order);
            }
        }

        private void Install(InstallPackageOrder order)
        {
            string packageId = order.PackageId;

            var repository = PackageRepositoryFactory.Default.CreateRepository(order.FeedUri);
            
            var manager = new PackageManager(repository, _destination);

            if (order.Version == null)
            {
                manager.InstallPackage(packageId);
            }
            else
            {
                manager.InstallPackage(packageId, new SemanticVersion(order.Version));
            }
        }
    }
}
