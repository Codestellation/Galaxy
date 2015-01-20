using Codestellation.Galaxy.ServiceManager.Operations;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager
{
    public class DeleteFolders : IOperation
    {
        private readonly string[] _folders;

        public DeleteFolders(string[] folders)
        {
            _folders = folders;
        }

        public void Execute(DeploymentTaskContext context)
        {
            foreach (var folder in _folders)
            {
                try
                {
                    context.BuildLog.Write("Delete folder {0} ", folder);
                    Folder.EnsureDeleted(folder);
                    context.BuildLog.WriteLine("succed.");
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