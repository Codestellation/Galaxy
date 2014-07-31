using System.Configuration;
using Topshelf;
using Topshelf.Logging;

namespace Codestellation.Galaxy
{
    internal static class Program
    {
        private const string ServiceName = "Codestellation.Galaxy";
        private const string Description = "Hosting service";

        private static int Main()
        {
            TopshelfExitCode code = HostFactory.Run(x =>
            {
                x.UseNLog();

                x.Service<Service>(s =>
                {
                    s.ConstructUsing(name => new Service());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                    s.WhenShutdown(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
                x.EnableShutdown();

                x.SetDescription(Description);

                x.SetDisplayName(ServiceName);
                x.SetServiceName(ServiceName);

                string instanceName = ConfigurationManager.AppSettings["InstanceName"];

                if (!string.IsNullOrEmpty(instanceName))
                {
                    x.SetInstanceName(instanceName);
                }
            });

            HostLogger.Shutdown();
            return (int) code;
        }
    }
}