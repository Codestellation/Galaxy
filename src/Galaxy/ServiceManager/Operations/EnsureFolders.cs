using Codestellation.Galaxy.Domain;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class EnsureFolders : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            var folders = context.GetValue<ServiceFolders>(DeploymentTaskContext.Folders);

            foreach (string specialFolder in folders.ToArray())
            {
                if (Folder.Exists(specialFolder))
                {
                    context.BuildLog.WriteLine($"Folder '{specialFolder}' exists");
                    continue;
                }

                Folder.EnsureExists(specialFolder);

                context.BuildLog.WriteLine($"Folder {specialFolder}' was created");
            }
        }
    }
}