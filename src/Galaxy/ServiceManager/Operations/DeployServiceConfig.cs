using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeployServiceConfig : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Config))
            {
                context.BuildLog.WriteLine("Service config is not provided or empty. Operation Skipped.");
                return;
            }

            context.BuildLog.WriteLine("Found service config:");
            context.BuildLog.WriteLine(context.Config);

            var configFolder = context.Folders.Configs;

            var configPath = Path.Combine((string)configFolder, "config.json");

            context.BuildLog.WriteLine("Write service config to '{0}'", configPath);
            File.WriteAllText(configPath, context.Config);
            context.BuildLog.WriteLine("Config was successfully written");
        }
    }
}