using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class UninstallPackage : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            context.BuildLog.WriteLine("Delete folder {0}", (string)context.Folders.DeployFolder);
            Folder.EnsureDeleted((string)context.Folders.DeployFolder);
        }
    }
}