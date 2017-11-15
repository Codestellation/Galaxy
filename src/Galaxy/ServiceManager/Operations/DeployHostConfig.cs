using System.Dynamic;
using System.IO;
using Codestellation.Galaxy.Domain;
using Codestellation.Quarks.IO;
using Newtonsoft.Json;

namespace Codestellation.Galaxy.ServiceManager.Operations
{
    public class DeployHostConfig : IOperation
    {
        public void Execute(DeploymentTaskContext context)
        {
            if (!Folder.Exists((string) context.Folders.DeployFolder))
            {
                context.BuildLog.WriteLine($"Directory '{context.Folders.DeployFolder}' does not exist. Host config will not be deployed.");
                return;
            }

            dynamic hostConfig = new ExpandoObject();
            hostConfig.Configs = (string)context.Folders.Configs;
            hostConfig.Logs = (string)context.Folders.Logs;
            hostConfig.Data = (string)context.Folders.Data;

            var hostConfigString = JsonConvert.SerializeObject(hostConfig, Formatting.Indented);

            context.BuildLog.WriteLine("Generated host config");
            context.BuildLog.WriteLine(hostConfigString);

            WriteConfig(context.BuildLog, context.Folders.DeployFolder, hostConfigString);
        }

        private void WriteConfig(TextWriter buildLog, FullPath serviceFolder, string hostConfig)
        {
            var configPath = Path.Combine((string)serviceFolder, "host.config.json");

            buildLog.WriteLine("Write host config to '{0}'", configPath);
            File.WriteAllText(configPath, hostConfig);
            buildLog.WriteLine("Config successfully written");
        }
    }
}