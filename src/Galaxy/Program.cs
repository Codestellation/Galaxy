using System.IO;
using Codestellation.Galaxy.Host;
using NLog;
using NLog.Config;
using Topshelf;
using Topshelf.HostConfigurators;

namespace Codestellation.Galaxy
{
    internal static class Program
    {
        private static int Main()
        {
            return Run.Service<Service>(ConfigureNlog);
        }

        private static void ConfigureNlog(HostConfigurator configurator, Service service)
        {
            var path = Path.Combine(service.HostConfig.Configs.FullName, "nlog.config");
            LogManager.Configuration = new XmlLoggingConfiguration(path);
            configurator.UseNLog(LogManager.GetLogger(typeof(Run).FullName).Factory);

        }
    }
}