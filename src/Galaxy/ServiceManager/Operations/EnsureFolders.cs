using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class EnsureFolders : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            foreach (string specialFolder in context.Folders.ToArray())
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