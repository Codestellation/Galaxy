using System;
using System.Diagnostics;
using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.Infrastructure;
using NuGet;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallPackage: IOperation
    {
        private readonly string _destination;
        private readonly InstallPackageOrder[] _orders;
        private readonly FileList _skipList;
        private TextWriter _buildLog;

        public InstallPackage(string destination, InstallPackageOrder[] orders, FileList skipList)
        {
            _destination = destination;
            _orders = orders;
            _skipList = skipList;
        }

        public InstallPackage(string destination, InstallPackageOrder[] orders) : this(destination, orders, FileList.Empty)
        {
            
        }

        public void Execute(TextWriter buildLog)
        {
            _buildLog = buildLog;
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

            _buildLog.WriteLine("Install '{0}' from '{1}' to '{2}'.", packageId, order.FeedUri, _destination);

            manager.PackageInstalled += MoveFilesToDestination;

            if (order.Version == null)
            {
                manager.InstallPackage(packageId);
            }
            else
            {
                manager.InstallPackage(packageId, new SemanticVersion(order.Version));
            }
        }

        private void MoveFilesToDestination(object sender, PackageOperationEventArgs e)
        {
            var installPath = e.InstallPath;
            _buildLog.WriteLine("Installed '{0}.{1}' to '{2}'.", e.Package.Id, e.Package.Version, installPath);
            
            foreach (var file in e.Package.GetFiles())
            {
                MoveFileToDestination(installPath, file);
            }

            _buildLog.WriteLine("Delete '{0}'", installPath);
            Folder.Delete(installPath);
        }

        private void MoveFileToDestination(string installPath, IPackageFile file)
        {
            var origin = Path.Combine(installPath, file.Path);
            var destination = Path.Combine(_destination, file.EffectivePath);
            try
            {
                _buildLog.Write("Copy '{0}' to '{1}'. ", origin, destination);

                if (_skipList.IsMatched(destination))
                {
                    _buildLog.WriteLine("Skipped.");
                    return;
                }

                if (File.Exists(destination))
                {
                    File.Delete(destination);
                }
                
                File.Move(origin, destination);
                _buildLog.WriteLine("Ok");
            }
            catch (Exception ex)
            {
                _buildLog.WriteLine("Failed: {0}", ex);
                throw;
            }
        }
    }
}
