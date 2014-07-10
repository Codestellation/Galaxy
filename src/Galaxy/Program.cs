using System;
using System.Configuration;
using NLog;
using Topshelf;

namespace Codestellation.Galaxy
{
    internal static class Program
    {
        private const string ServiceName = "Codestellation.Galaxy";
        private const string Description = "Hosting service";

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            HostFactory.Run(x =>
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
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            Log.FatalException("Unhandled exception.", exception);
            LogManager.Flush(3000);
        }
    }
}