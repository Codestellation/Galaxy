using System;
using Codestellation.Galaxy.Host.ConfigManagement;

namespace Codestellation.Galaxy.Host
{
    internal class ServiceProxy
    {
        public ServiceProxy(Type serviceType, HostConfig hostConfig)
        {
            Service = (IService)Activator.CreateInstance(serviceType);

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