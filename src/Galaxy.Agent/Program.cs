using System.IO;
using Codestellation.Galaxy.Host;
using Codestellation.Quarks.IO;
using NLog;
using NLog.Config;

namespace Codestellation.Galaxy.Agent
{
    internal class Program
    {
        private static void Main()
        {
            var nlogPath = Folder.Combine("data", "nlog.config");

            if (File.Exists(nlogPath))
            {
                LogManager.Configuration = new XmlLoggingConfiguration(nlogPath);
            }

            Run.Service<AgentService>();
        }
    }
}