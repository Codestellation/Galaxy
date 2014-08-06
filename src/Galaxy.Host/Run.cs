using System;
using System.IO;
using System.Xml.Serialization;
using Topshelf;
using Topshelf.Logging;

namespace Codestellation.Galaxy.Host
{
    public static class Run
    {
        public static int Service<TService>()
        {
            Type serviceType = typeof (TService);

            TopshelfExitCode code = HostFactory.Run(x =>
            {
                var config = GetSettings();

                x.UseNLog();

                x.Service<ServiceProxy>(s =>
                {
                    s.ConstructUsing(name => new ServiceProxy(serviceType, config));
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
            return (int)code;
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