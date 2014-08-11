using System;
using System.Linq;
using System.Reflection;
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
                x.UseNLog();

                x.Service<ServiceProxy>(s =>
                {
                    s.ConstructUsing(name => new ServiceProxy(serviceType));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                    s.WhenShutdown(tc => tc.Stop());
                });

                x.EnableShutdown();
                x.RunAsLocalSystem();

                Assembly serviceAssembly = Assembly.GetEntryAssembly();
                string serviceName = GetServiceName(serviceAssembly);
                x.SetServiceName(serviceName);
                x.SetDisplayName(serviceName.Replace('.', ' '));
                x.SetDescription(GetServiceDescription(serviceAssembly));
            });

            HostLogger.Shutdown();
            return (int)code;
        }

        static string GetServiceName(Assembly assembly)
        {
            AssemblyName assemblyName = assembly.GetName();
            return assemblyName.Name;
        }

        static string GetServiceDescription(Assembly assembly)
        {
            AssemblyDescriptionAttribute description = assembly
                .GetCustomAttributes(typeof (AssemblyDescriptionAttribute), false)
                .Cast<AssemblyDescriptionAttribute>()
                .FirstOrDefault();

            return description != null ? description.Description : null;
        }
    }
}