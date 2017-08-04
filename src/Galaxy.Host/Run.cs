using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Codestellation.Galaxy.Host.LogManagement;
using Codestellation.Galaxy.Host.Misc;
using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.Logging;

namespace Codestellation.Galaxy.Host
{
    public static class Run
    {
        private static ServiceProxy _serviceProxy;
        private static bool _startService = true;

        public static int Service<TService>(Action<HostConfigurator, TService> options = null)
            where TService : IService, new()
        {
            Type serviceType = typeof(TService);

            _serviceProxy = new ServiceProxy(serviceType);

            ProcessCustomCommand();

            if (!_startService)
            {
                return 0;
            }

            _serviceProxy.SetupService();

            if (options == null)
            {
                options = (configurator, config) => { };
            }

            TopshelfExitCode code = RunServiceNormally<TService>(serviceType, options);

            HostLogger.Shutdown();
            return (int)code;
        }

        public static void ProcessCustomCommand()
        {
            if (Environment.GetCommandLineArgs().Contains("config-sample"))
            {
                string sample = _serviceProxy.GetConfigSample();

                Console.Out.WriteLine(sample);
                _startService = false;
            }
        }

        private static TopshelfExitCode RunServiceNormally<TService>(Type serviceType, Action<HostConfigurator, TService> options)
            where TService : IService, new()
        {
            TopshelfExitCode code = HostFactory.Run(x =>
            {
                options(x, (TService)_serviceProxy.Service);
                x.InitializeLoggers(serviceType.Assembly, _serviceProxy.HostConfig.Configs);
                VersionLogger.LogVersions();

                x.Service<ServiceProxy>(s =>
                {
                    s.ConstructUsing(name => _serviceProxy);
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
            return code;
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

        private static string GetServiceName(Assembly assembly)
        {
            AssemblyName assemblyName = assembly.GetName();
            return assemblyName.Name;
        }

        private static string GetServiceDescription(Assembly assembly)
        {
            return assembly.GetAttribute<AssemblyDescriptionAttribute>()?.Description;
        }
    }
}