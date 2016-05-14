using Codestellation.Galaxy.Domain;
using Codestellation.Quarks.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class EnsureFolders : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            var folders = context.GetValue<SpecialFolder[]>(DeploymentTaskContext.Folders);

            foreach (var specialFolder in folders)
            {
                if (Folder.Exists(specialFolder.FullPath))
                {
                    context.BuildLog.WriteLine($"Folder '{specialFolder.Purpose}'-'{specialFolder.FullPath}' exists");
                    continue;
                }

                Folder.EnsureExists(specialFolder.FullPath);

                context.BuildLog.WriteLine($"Folder '{specialFolder.Purpose}'-'{specialFolder.FullPath}' was created");
            }
        }
    }
}