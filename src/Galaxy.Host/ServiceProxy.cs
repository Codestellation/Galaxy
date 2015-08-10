using System;

namespace Codestellation.Galaxy.Host
{
    internal class ServiceProxy
    {
        private readonly IService _instance;

        public ServiceProxy(Type serviceType)
        {
            _instance = (IService)Activator.CreateInstance(serviceType);
        }

        public void Start()
        {
            _instance.Start();
        }

        public void Stop()
        {
            _instance.Stop();
        }
    }
}