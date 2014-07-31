using System;
using System.IO;
using System.Xml.Serialization;
using Topshelf;
using Topshelf.Logging;

namespace Codestellation.Galaxy.Host
{
    internal static class Program
    {
        private static int Main()
        {
            TopshelfExitCode code = HostFactory.Run(x =>
            {
                x.UseNLog();

                var config = GetSettings();

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

                if (!string.IsNullOrEmpty(config.InstanceName))
                {
                    x.SetInstanceName(config.InstanceName);
                }
            });

            HostLogger.Shutdown();
            return (int) code;
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
    }
}
