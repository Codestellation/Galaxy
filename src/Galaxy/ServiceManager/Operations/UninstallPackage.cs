using System.IO;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class UninstallPackage : IOperation
    {
        private readonly string _serviceFolder;

        public UninstallPackage(string serviceFolder)
        {
            _serviceFolder = serviceFolder;
        }

        public void Execute(DeploymentTaskContext context)
        {
            context.BuildLog.WriteLine("Delete folder {0}", _serviceFolder);
            Folder.EnsureDeleted(_serviceFolder);
        }
    }
}
