using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeleteFolders : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            foreach (var folder in context.Folders.ToArray())
            {
                try
                {
                    context.BuildLog.Write("Delete folder {0} ", folder);
                    Folder.EnsureDeleted(folder);
                    context.BuildLog.WriteLine("succeed.");
                }
                catch
                {
                    context.BuildLog.WriteLine("failed.");
                    throw;
                }
            }
        }
    }
}