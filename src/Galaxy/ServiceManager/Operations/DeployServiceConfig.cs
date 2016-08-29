using System.IO;
using System.Linq;
using Codestellation.Galaxy.Domain;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeployServiceConfig : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            string config;
            if (!context.TryGetValue<string>(DeploymentTaskContext.Config, out config) || string.IsNullOrWhiteSpace(config))
            {
                context.BuildLog.WriteLine("Service config is not provided or empty. Operation Skipped.");
                return;
            }

            context.BuildLog.WriteLine("Found service config:");
            context.BuildLog.WriteLine(config);

            var configFolder = context
                .GetValue<SpecialFolder[]>(DeploymentTaskContext.Folders)
                .Single(x => x.Purpose == SpecialFolderDictionary.Configs);

            var configPath = Path.Combine(configFolder.FullPath, "config.json");

            context.BuildLog.WriteLine("Write service config to '{0}'", configPath);
            File.WriteAllText(configPath, config);
            context.BuildLog.WriteLine("Config was successfully written");
        }
    }
}