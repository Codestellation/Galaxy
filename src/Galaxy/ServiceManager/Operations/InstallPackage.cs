using System;
using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Quarks.IO;
using NuGet;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class InstallPackage : IOperation
    {
        private readonly string _destination;
        private readonly PackageDetails _packageDetails;
        private readonly FileList _skipList;
        private TextWriter _buildLog;

        public InstallPackage(string destination, PackageDetails packageDetails, FileList skipList)
        {
            _destination = destination;
            _packageDetails = packageDetails;
            _skipList = skipList;
        }

        public InstallPackage(string destination, PackageDetails details)
            : this(destination, details, FileList.Empty)
        {
        }

        public void Execute(DeploymentTaskContext context)
        {
            _buildLog = context.BuildLog;
            Install();
        }

        private void Install()
        {
            string packageId = _packageDetails.PackageId;

            var repository = PackageRepositoryFactory.Default.CreateRepository(_packageDetails.FeedUri);

            var manager = new PackageManager(repository, _destination);

            _buildLog.WriteLine("Install '{0}' from '{1}' to '{2}'.", packageId, _packageDetails.FeedUri, _destination);

            manager.PackageInstalled += MoveFilesToDestination;

            if (_packageDetails.Version == null)
            {
                manager.InstallPackage(packageId);
            }
            else
            {
                manager.InstallPackage(packageId, new SemanticVersion(_packageDetails.Version));
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
            Folder.EnsureDeleted(installPath);
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

                var info = new FileInfo(destination);
                Folder.EnsureExists(info.DirectoryName);

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