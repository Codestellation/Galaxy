using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Topshelf;
using Topshelf.Logging;

namespace Codestellation.Galaxy.Host
{
    public static class Run
    {
        private static char[] InvalidCharacters = {' ', '/', '\\'};

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

                var serviceName = config.ServiceName;
                ValidateServiceName(serviceName);
                x.SetServiceName(serviceName);

                var instanceName = config.InstanceName;
                if (!string.IsNullOrEmpty(instanceName))
                {
                    ValidateServiceName(instanceName);
                    x.SetInstanceName(instanceName);
                }

                x.SetDescription(config.Description);
                x.SetDisplayName(config.DisplayName);
            });

            HostLogger.Shutdown();
            return (int)code;
        }

        private static void ValidateServiceName(string name)
        {
            if (name.Intersect(InvalidCharacters).Any())
            {
                var message = "Service or instance name contains invalid characters: space, '/', '\'";
                throw new InvalidOperationException(message);
            }
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