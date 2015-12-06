using System;
using Codestellation.Galaxy.Host.ConfigManagement;

namespace Codestellation.Galaxy.Host
{
    internal class ServiceProxy
    {
        public ServiceProxy(Type serviceType)
        {
            Service = (IService)Activator.CreateInstance(serviceType);

            var hostConfig = HostConfig.Load();

            hostConfig.Validate();

            Service.HostConfig = hostConfig;

            ConfigManager.TryLoadConfig(Service);
        }

        public IService Service { get; }

        public void Start()
        {
            Service.Start();
        }

        public void Stop()
        {
            Service.Stop();
        }
    }
}