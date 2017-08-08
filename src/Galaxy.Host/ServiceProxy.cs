using System;
using Codestellation.Galaxy.Host.ConfigManagement;

namespace Codestellation.Galaxy.Host
{
    public class ServiceProxy : MarshalByRefObject
    {
        public IService Service { get; }

        public ServiceProxy(Type serviceType, HostConfig hostConfig)
            : this(serviceType)
        {
            Service.HostConfig = hostConfig;
        }

        public ServiceProxy(Type serviceType)
        {
            Service = (IService)Activator.CreateInstance(serviceType);
        }

        public void SetupService()
        {
            if (Service.HostConfig == null)
            {
                var hostConfig = HostConfig.Load();
                hostConfig.Validate();
                Service.HostConfig = hostConfig;
            }
            ConfigManager.TryLoadConfig(Service);
        }

        public void Start()
        {
            Service.Start();
        }

        public void Stop()
        {
            Service.Stop();
        }

        public string GetConfigSample()
        {
            return ConfigManager.GetSample(Service);
        }
    }
}