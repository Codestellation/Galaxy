using System;
using System.IO;
using System.Xml.Serialization;
using NLog;
using Topshelf;

namespace Codestellation.Galaxy.Host
{
    internal static class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            var config = GetSettings();

            HostFactory.Run(x =>
                {
                    x.UseNLog();

                    x.Service<ServiceProxy>(s =>
                        {
                            s.ConstructUsing(name => new ServiceProxy(config));
                            s.WhenStarted(tc => tc.Start());
                            s.WhenStopped(tc => tc.Stop());
                            s.WhenShutdown(tc => tc.Stop());
                        });

                    x.EnableShutdown();
                    x.RunAsLocalSystem();

                    x.SetDescription(config.Description);
                    
                    x.SetServiceName(config.ServiceName);
                    x.SetDisplayName(config.DisplayName);

                    string instanceName = string.Empty;

                    if (!string.IsNullOrEmpty(instanceName))
                    {
                        x.SetInstanceName(instanceName);
                    }
                });
        }

        private static ServiceConfig GetSettings()
        {
            var serializer = new XmlSerializer(typeof(ServiceConfig));

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "service-config.xml");

            using (var stream = File.OpenRead(path))
            {
                return (ServiceConfig)serializer.Deserialize(stream);
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            Log.Fatal("Unhandled exception.", exception);
            LogManager.Flush(3000);
        }
    }
}
