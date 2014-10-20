using System;
using System.Threading;
using Codestellation.Galaxy.Configuration;
using Codestellation.Quarks.IO;
using NLog;
using NLog.Config;
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
            var nlogPath = Folder.Combine("data", "nlog.config");
            LogManager.Configuration = new XmlLoggingConfiguration(nlogPath);
            
            var configuration = ServiceConfig.Read();

            TopshelfExitCode code = HostFactory.Run(x =>
            {
                x.UseNLog();

                x.Service<Service>(s =>
                {
                    s.ConstructUsing(name => new Service(configuration));
                    s.WhenStarted(StartService);
                    s.WhenStopped(tc => tc.Stop());
                    s.WhenShutdown(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
                x.EnableServiceRecovery(ConfigureRecovery); 
                x.EnableShutdown();

                x.SetDescription(Description);

                x.SetDisplayName(ServiceName);
                x.SetServiceName(ServiceName);

                string instanceName = configuration.InstanceName;

                if (!string.IsNullOrEmpty(instanceName))
                {
                    x.SetInstanceName(instanceName);
                }
            });

            HostLogger.Shutdown();
            return (int) code;
        }

        private static bool StartService(Service service, HostControl hostControl)
        {
            return ThreadPool.UnsafeQueueUserWorkItem(StartServiceAsync, service);
        }

        private static void StartServiceAsync(object state)
        {
            var service = (Service)state;

            try
            {
                service.Start();
            }
            catch (Exception ex)
            {
                HostLogger.Get(typeof(Program)).Fatal("Service could not start", ex);
                HostLogger.Shutdown();
                throw;
            }
        }

        private static void ConfigureRecovery(ServiceRecoveryConfigurator configurator)
        {
            const int oneMinute = 1;
            const int daily = 1;
            configurator.RestartService(oneMinute);
            configurator.SetResetPeriod(daily);
        }
    }
}