using System.IO;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeployServiceConfig : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            if (!context.TryGetValue(DeploymentTaskContext.Config, out string config)
                || string.IsNullOrWhiteSpace(config))
            {
                context.BuildLog.WriteLine("Service config is not provided or empty. Operation Skipped.");
                return;
            }

            context.BuildLog.WriteLine("Found service config:");
            context.BuildLog.WriteLine(config);

            var configFolder = context.Folders.Configs;

            var configPath = Path.Combine((string)configFolder, "config.json");

            context.BuildLog.WriteLine("Write service config to '{0}'", configPath);
            File.WriteAllText(configPath, config);
            context.BuildLog.WriteLine("Config was successfully written");
        }
    }
}