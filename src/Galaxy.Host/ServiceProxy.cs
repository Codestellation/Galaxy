using System;
using Codestellation.Galaxy.Host.ConfigManagement;

namespace Codestellation.Galaxy.Host
{
    internal class ServiceProxy
    {
        private readonly IService _service;

        public ServiceProxy(Type serviceType)
        {
            _service = (IService)Activator.CreateInstance(serviceType);

            ConsulConfigManager.TryLoadConsulConfig(_service);
        }

        public IService Service
        {
            get { return _service; }
        }

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