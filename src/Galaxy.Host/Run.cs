using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using NLog;
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
                x.UseNLog(LogManager.GetCurrentClassLogger().Factory);

                LogVersions();

                x.Service<ServiceProxy>(s =>
                {
                    s.ConstructUsing(name => new ServiceProxy(serviceType));
                    s.WhenStarted(StartService);
                    s.WhenStopped(tc => tc.Stop());
                    s.WhenShutdown(tc => tc.Stop());
                });

                x.EnableShutdown();
                x.EnableServiceRecovery(ConfigureRecovery); 
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

        private static void LogVersions()
        {
            var logWriter = HostLogger.Get(typeof (Run));
            
            var hostVersion = GetVersion(Assembly.GetExecutingAssembly());
            var hostVersionMessage = string.Format("Host version: {0}", hostVersion);
            logWriter.Info(hostVersionMessage);

            var serviceVersion = GetVersion(Assembly.GetEntryAssembly());
            var serviceVersionMessage = string.Format("Service version: {0}", serviceVersion);
            logWriter.Info(serviceVersionMessage);
        }

        private static bool StartService(ServiceProxy service, HostControl hostControl)
        {
            return ThreadPool.UnsafeQueueUserWorkItem(StartServiceAsync, service);
        }

        private static void StartServiceAsync(object state)
        {
            var service = (ServiceProxy)state;

            try
            {
                service.Start();
            }
            catch (Exception ex)
            {
                HostLogger.Get(typeof(Run)).Fatal("Service could not start", ex);
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

        static string GetServiceName(Assembly assembly)
        {
            AssemblyName assemblyName = assembly.GetName();
            return assemblyName.Name;
        }

        static string GetServiceDescription(Assembly assembly)
        {
            var description = GetAttribute<AssemblyDescriptionAttribute>(assembly);

            return description == null ? null : description.Description;
        }

        private static string GetVersion(Assembly assembly)
        {
            var info = GetAttribute<AssemblyInformationalVersionAttribute>(assembly);

            if (info != null)
            {
                return info.InformationalVersion;
            }

            var version = GetAttribute<AssemblyVersionAttribute>(assembly);
            if (version != null)
            {
                return version.Version;
            }
            return "Unknown";
        }

        private static T GetAttribute<T>(Assembly assembly)
        {
            return assembly
                   .GetCustomAttributes(typeof(T), false)
                   .Cast<T>()
                   .FirstOrDefault();
            
        }
    }
}